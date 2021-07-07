using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using HLab.Base;
using HLab.Core.Annotations;

namespace HLab.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class Bootstrapper
    {

        private class Context : IBootContext
        {
            private readonly Bootstrapper _bootstrapper;
            private readonly Action<IBootContext> _action;
            public string Name { get; }

            public Context(Bootstrapper bootstrapper, string name, Action<IBootContext> action)
            {
                _bootstrapper = bootstrapper;
                _action = action;
                Name = name;
            }

            public void Requeue()
            {
                _bootstrapper.Enqueue(this);
            }

            public void Enqueue(string name, Action<IBootContext> action)
            {
                _bootstrapper.Enqueue(name, action);
            }

            public void Invoke() => _action(this);

            public bool StillContains(params string[] name) => _bootstrapper.Contains(name);
            public override string ToString() => Name;
        }

        private readonly Func<IEnumerable<IBootloader>> _getBootloaders;

        public Bootstrapper(Func<IEnumerable<IBootloader>> getBootloaders)
        {
            _getBootloaders = getBootloaders;
        }



        private readonly ConcurrentQueue<Context> _queue = new();
        public void Boot()
        {
            var bootLoaders = Sort(_getBootloaders());

            HashSet<string> done = new();

            foreach (var bootLoader in bootLoaders)
            {
                var name = bootLoader.GetType().FullName;
                if (done.Contains(name)) continue;
                Enqueue(name, bs => bootLoader.Load(bs));
                done.Add(name);
            }

            while (_queue.TryDequeue(out var context))
            {
                context.Invoke();
            }
        }

        private static IEnumerable<T> Sort<T>(IEnumerable<T> src)
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



        private void Enqueue(Context context)
        {
            _queue.Enqueue(context);
        }

        public void Enqueue(string name, Action<IBootContext> action)
        {
            Enqueue(new Context(this, name, action));
        }

        public bool Contains(params string[] names)
        {
            return names.Any(name => _queue.Any(e => e.Name.EndsWith(name)));
        }

    }
}
