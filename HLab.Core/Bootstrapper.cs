using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HLab.Base;
using HLab.Core.Annotations;

namespace HLab.Core;

/// <summary>
/// 
/// </summary>
public class Bootstrapper(Func<IEnumerable<IBootloader>> getBootloaders)
{
    class Context(Bootstrapper bootstrapper, string name, Func<IBootContext,Task> action) : IBootContext
    {
        public string Name => name;


        public void Requeue() => bootstrapper.Enqueue(this);

        public void Enqueue(string name, Func<IBootContext,Task> a) => bootstrapper.Enqueue(name, a);

        public Task InvokeAsync() => action(this);

        public bool StillContains(params string[] name) => bootstrapper.Contains(name);

        public override string ToString() => Name;
    }

    readonly ConcurrentQueue<Context> _queue = new();
    public async Task BootAsync()
    {
        var bl = getBootloaders();

        var bootLoaders = Sort(bl);

        HashSet<string> done = new();

        foreach (var bootLoader in bootLoaders)
        {
            var name = bootLoader.GetType().FullName;
            if (done.Contains(name)) continue;
            Enqueue(name, bs => bootLoader.LoadAsync(bs));
            done.Add(name);
        }

        while (_queue.TryDequeue(out var context)) 
            await context.InvokeAsync();
    }

    static IEnumerable<T> Sort<T>(IEnumerable<T> src)
    {
        var result = new List<T>();
        foreach (var boot in src)
        {
            var bootAssemblyName = boot.GetType().Assembly.GetName().Name;
            for (var i = 0; i < result.Count; i++)
            {
                var a = result[i].GetType().Assembly;
                if (a.References(bootAssemblyName))
                {
                    result.Insert(i, boot);
                    goto inserted;
                }
            }
            result.Add(boot);
            inserted:;
        }

        return result;
    }

    //private readonly Dictionary<string, Assembly> _loadedAssemblies = new Dictionary<string, Assembly>();

    void Enqueue(Context context) 
        => _queue.Enqueue(context);

    public void Enqueue(string name, Func<IBootContext,Task> action) 
        => Enqueue(new Context(this, name, action));

    public bool Contains(params string[] names) 
        => names.Any(name => _queue.Any(e => e.Name.EndsWith(name)));
}