using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace HLab.Compiler.Wpf
{
    public class CompileError
    {
        public string Message { get; set; }
        public int Line { get; set; }
        public int Pos { get; set; }
        public int Length { get; set; }
    }

    public static class Compiler
    {
        public static async Task<byte[]> CompileAsync(IList<CompileError> errors, params string[] sources)
        {
            byte[] binary = null;
            EmitResult result = null;

            await Task.Run(() =>
            {
                using var peStream = new MemoryStream();

                result = Compiler.GenerateCode(sources).Emit(peStream);
                if (!result.Success) return;

                peStream.Seek(0, SeekOrigin.Begin);

                binary = peStream.ToArray();
            });

            var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

            foreach (var failure in failures)
            {
                var span = failure.Location.GetLineSpan().Span;

                var err = new CompileError
                {
                    Message = failure.GetMessage(),
                    Line = span.Start.Line + 1,
                    Pos = span.Start.Character + 1,
                    Length = span.End.Character - span.Start.Character
                };
                errors.Add(err);
            }

            return binary;

        }

        public static Assembly Load(byte[] compiled)
        {
            var assemblyLoadContext = new AssemblyLoadContext("HLab.RuntimeCompiled", true);

            using var asm = new MemoryStream(compiled);
            return assemblyLoadContext.LoadFromStream(asm);
        }

        static CSharpCompilation GenerateCode(string[] sources)
        {
            List<SyntaxTree> parsedSyntaxTrees = new();
            foreach (var source in sources)
            {
                var codeString = SourceText.From(source);
                var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);

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

        public static void Execute(byte[] compiledAssembly, string[] args)
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
        static WeakReference LoadAndExecute(byte[] compiledAssembly, string[] args)
        {
            using var asm = new MemoryStream(compiledAssembly);
            var assemblyLoadContext = new AssemblyLoadContext("HLab.RuntimeCompiled", true);

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

