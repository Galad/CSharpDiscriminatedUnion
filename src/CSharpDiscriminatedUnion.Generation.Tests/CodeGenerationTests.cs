//using CodeGeneration.Roslyn;
//using DiscriminatedUnion.Generation.Attributes;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using NUnit.Framework;
//using System;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Validation;

//namespace DiscriminatedUnion.Generation.Tests
//{
//    public class CodeGenerationTests
//    {
//        private static CSharpCompilation CreateCompilation(IEnumerable<SyntaxTree> syntaxTrees)
//        {
//            var compilation = CSharpCompilation.Create("codegen", syntaxTrees)
//                                               .WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary))
//                                               .AddReferences(MetadataReference.CreateFromFile(@"C:\Users\galad\.nuget\packages\system.collections.immutable\1.1.37\lib\portable-net45+win8+wp8+wpa81\System.Collections.Immutable.dll"))
//                                               .AddReferences(MetadataReference.CreateFromFile(typeof(ValueTuple).Assembly.Location))
//                                               .AddReferences(MetadataReference.CreateFromFile(typeof(string).Assembly.Location))
//                                               .AddReferences(MetadataReference.CreateFromFile(typeof(Attribute).Assembly.Location))
//                                               //.AddReferences(MetadataReference.CreateFromFile(typeof(ImmutableArray).Assembly.Location))
//                                               .AddReferences(MetadataReference.CreateFromFile(typeof(GenerateUnionAttribute).Assembly.Location))
//                                               .AddReferences(MetadataReference.CreateFromFile(typeof(CodeGenerationAttributeAttribute).Assembly.Location))
//                                               .AddReferences(MetadataReference.CreateFromFile(typeof(CodeGenerator).Assembly.Location));
//            return compilation;
//        }

//        string inputSource =
//            @"
//using DiscriminatedUnion.Generation.Attributes;

//namespace DiscriminatedUnion.Generation.Tests
//{    
//    public partial class SingleCaseUnion
//    {
//        private static partial class Cases
//        {
//            partial class SingleCase : SingleCaseUnion
//            {
//            }
//        }
//    }
//}
//            ";

//        [Test]
//        public void TestCodeGeneration()
//        {
//            var syntaxTree = CSharpSyntaxTree.ParseText(inputSource);
//            var compilation = CreateCompilation(new[] { syntaxTree });
//            //var generatorDiagnostics = new List<Diagnostic>();
//            //var progress = new SynchronousProgress<Diagnostic>(generatorDiagnostics.Add);
//            //var s = compilation.SyntaxTrees.Single();
//            //DocumentTransform.TransformAsync(compilation, s, progress).Wait();
//        }

//        private class SynchronousProgress<T> : IProgress<T>
//        {
//            private readonly Action<T> action;

//            public SynchronousProgress(Action<T> action)
//            {
//                Requires.NotNull(action, nameof(action));

//                this.action = action;
//            }

//            public void Report(T value)
//            {
//                this.action(value);
//            }
//        }
//    }
//}
