using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Windows;
using System.Windows.Controls;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace Compiler.Wpf
{
    public class Compiler
    {
        public string SourceCode { get; set; }
        public string CsMessage { get; private set; }
        public object Module { get; private set; }

        public bool Compile()
        {
            var sourceCode = SourceCode;

            using var peStream = new MemoryStream();

            var result = GenerateCode(sourceCode).Emit(peStream);

            if (!result.Success)
            {
                CsMessage = "Compilation failed.";

                var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);

                foreach (var diagnostic in failures)
                {
                    CsMessage += $"\n{diagnostic.Id}: {diagnostic.GetMessage()}";
                }

                return false;
            }

            CsMessage = "C# OK";

            peStream.Seek(0, SeekOrigin.Begin);

            var compiled = peStream.ToArray();
            var assemblyLoadContext = new AssemblyLoadContext("HLab.RuntimeCompiled", true);

            Assembly assembly;
            using (var asm = new MemoryStream(compiled))
            {
                assembly = assemblyLoadContext.LoadFromStream(asm);
            }

            Module = Activator.CreateInstance(assembly.GetTypes()[0]);

            return true;
        }

        private static CSharpCompilation GenerateCode(string sourceCode)
        {
            var codeString = SourceText.From(sourceCode);
            var options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.CSharp7_3);

            var parsedSyntaxTree = SyntaxFactory.ParseSyntaxTree(codeString, options);

            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                .Select(a => MetadataReference.CreateFromFile(a.Location))
                .ToArray();

            //var references = new MetadataReference[]
            //{
            //    //System.Runtime
            //    MetadataReference.CreateFromFile(typeof(System.Runtime.AssemblyTargetedPatchBandAttribute).Assembly.Location),
            //    //System.Private.CoreLib
            //    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            //    //PresentationCore
            //    MetadataReference.CreateFromFile(typeof(UIElement).Assembly.Location),
            //    //PresentationFramework
            //    MetadataReference.CreateFromFile(typeof(TextBox).Assembly.Location),
            //    //WindowBase
            //    MetadataReference.CreateFromFile(typeof(DependencyObject).Assembly.Location),
            //    //HLab.Base.Wpf
            //    MetadataReference.CreateFromFile(typeof(TextBoxEx).Assembly.Location),
            //    //HLab.Erp.Lims.Analysis.Module
            //    MetadataReference.CreateFromFile(typeof(IFormTarget).Assembly.Location),
            //    MetadataReference.CreateFromFile(typeof(IForm).Assembly.Location),
            //    //System.ObjectModel
            //    MetadataReference.CreateFromFile(typeof(INotifyPropertyChanged).Assembly.Location),
            //    MetadataReference.CreateFromFile(typeof(INotifyPropertyChangedWithHelper).Assembly.Location),
            //    //HLab.Notify.PropertyChanged
            //    MetadataReference.CreateFromFile(typeof(NotifierBase).Assembly.Location),
            //    //HLab.Notify.PropertyChanged
            //    MetadataReference.CreateFromFile(typeof(UserControlNotifier).Assembly.Location),
            //    //

            //    MetadataReference.CreateFromFile(Assembly.GetExecutingAssembly().Location),
            //    MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location),
            //    MetadataReference.CreateFromFile(Assembly.Load("netstandard").Location),
            //    MetadataReference.CreateFromFile(Assembly.Load("System.Linq").Location),
            //    MetadataReference.CreateFromFile(Assembly.Load("System.Core").Location),
            //    MetadataReference.CreateFromFile(Assembly.Load("System.Xaml").Location),
            //    MetadataReference.CreateFromFile(Assembly.Load("System.ComponentModel.Primitives").Location),
            //};

            return CSharpCompilation.Create("HLab.RuntimeCompiled.dll",
                new[] { parsedSyntaxTree },
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

