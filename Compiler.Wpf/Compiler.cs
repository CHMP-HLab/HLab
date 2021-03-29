using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Compiler.Wpf
{
    public class Compiler
    {
        public static Assembly Compile(out string messages, params string[] sources)
        {
            using var peStream = new MemoryStream();

            var result = GenerateCode(sources).Emit(peStream);

            if (!result.Success)
            {
                messages = "Compilation failed.";

                var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (var diagnostic in failures)
                {
                    messages += $"\n{diagnostic.Id}: {diagnostic.GetMessage()}";
                }

                return null;
            }

            messages = "C# OK";

            peStream.Seek(0, SeekOrigin.Begin);

            var compiled = peStream.ToArray();
            var assemblyLoadContext = new AssemblyLoadContext("HLab.RuntimeCompiled", true);

            using var asm = new MemoryStream(compiled);
            return assemblyLoadContext.LoadFromStream(asm);
        }

        private static CSharpCompilation GenerateCode(string[] sources)
        {
            List<SyntaxTree> parsedSyntaxTrees = new();
            foreach (var source in sources)
            {
                var codeString = SourceText.From(source);
                var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3);

                parsedSyntaxTrees.Add(SyntaxFactory.ParseSyntaxTree(codeString, options));
            }

            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                /*.ToArray()*/;


            return CSharpCompilation.Create("HLab.RuntimeCompiled.dll",
                parsedSyntaxTrees,
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }

        public void Execute(byte[] compiledAssembly, string[] args)
        {
            var assemblyLoadContextWeakRef = LoadAndExecute(compiledAssembly, args);

            for (var i = 0; i < 8 && assemblyLoadContextWeakRef.IsAlive; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            Console.WriteLine(assemblyLoadContextWeakRef.IsAlive ? "Unloading failed!" : "Unloading success!");
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static WeakReference LoadAndExecute(byte[] compiledAssembly, string[] args)
        {
            using var asm = new MemoryStream(compiledAssembly);
            var assemblyLoadContext = new AssemblyLoadContext("HLab.RuntimeCompiled",true);

            var assembly = assemblyLoadContext.LoadFromStream(asm);

            var entry = assembly.EntryPoint;

            _ = entry != null && entry.GetParameters().Length > 0
                ? entry.Invoke(null, new object[] { args })
                : entry.Invoke(null, null);

            assemblyLoadContext.Unload();

            return new WeakReference(assemblyLoadContext);
        }
    }
}

