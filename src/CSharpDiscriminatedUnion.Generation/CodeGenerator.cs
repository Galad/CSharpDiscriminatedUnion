using CodeGeneration.Roslyn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading;
using Validation;
using System.Diagnostics;
using System.Collections.Immutable;
using CSharpDiscriminatedUnion.Attributes;

namespace CSharpDiscriminatedUnion.Generation
{
    public class CodeGenerator : ICodeGenerator
    {
        private readonly string _caseFactoryPrefix;
        private readonly bool _preventNullValues;

        public CodeGenerator(AttributeData attributeData)
        {
            Requires.NotNull(attributeData, nameof(attributeData));
            var arguments = attributeData.NamedArguments.ToImmutableDictionary(kvp => kvp.Key, k => k.Value.Value);
            T GetAttributeValue<T>(string name, T defaultValue, T nullValue)
            {
                return (T)(arguments.GetValueOrDefault(name, defaultValue) ?? nullValue);
            }
            _caseFactoryPrefix = GetAttributeValue(nameof(GenerateDiscriminatedUnionAttribute.CaseFactoryPrefix), "New", "");
            _preventNullValues = GetAttributeValue(nameof(GenerateDiscriminatedUnionAttribute.PreventNullValues), false, false);            
        }
        
        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(MemberDeclarationSyntax applyTo, CSharpCompilation compilation, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
            var semanticModel = compilation.GetSemanticModel(applyTo.SyntaxTree);
            var applyToClass = applyTo as ClassDeclarationSyntax;
            if (applyToClass == null)
            {
                var structGenerator = new StructDiscriminatedUnionGenerator(_caseFactoryPrefix, _preventNullValues);                
                var applyToStruct = applyTo as StructDeclarationSyntax;
                if( applyToStruct == null)
                {
                    throw new InvalidOperationException("applyTo is not a ClassDeclarationSyntax nor a StructDeclarationSyntax");
                }
                var applyToStructSymbolInfo = semanticModel.GetDeclaredSymbol(applyToStruct) as INamedTypeSymbol;
                if (applyToStructSymbolInfo == null)
                {
                    throw new InvalidOperationException($"applyTo symbol has not been found");
                }
                var applyToStructType = GetTypeSyntax(applyToStruct, applyToStructSymbolInfo);
                var structCases = GetStructCases(applyToStruct, semanticModel);
                var partialStruct = GetPartialStruct(applyToStruct, applyToStructType);
                var structContext = new DiscriminatedUnionContext<StructDiscriminatedUnionCase>(
                        applyToStruct,
                        applyToStructType,
                        partialStruct,
                        semanticModel,
                        applyToStructSymbolInfo,
                        structCases
                    );
                var structResult = structGenerator.Build(structContext);
                var resultStruct = BuildStruct(structResult);
                var structs = List(new[] { (MemberDeclarationSyntax)resultStruct });
                return Task.FromResult(structs);
            }           
            var generator = new ClassDiscriminatedUnionGenerator(_caseFactoryPrefix, _preventNullValues);
            var applyToClassSymbolInfo = semanticModel.GetDeclaredSymbol(applyTo) as INamedTypeSymbol;
            if (applyToClassSymbolInfo == null)
            {
                throw new InvalidOperationException($"applyTo symbol has not been found");
            }
            var applyToClassType = GetTypeSyntax(applyToClass, applyToClassSymbolInfo);
            var partialClass = GetPartialClass(applyToClass, applyToClassType);
            var cases = GetCases(applyToClass, applyToClassSymbolInfo, semanticModel);

            var context = new DiscriminatedUnionContext<ClassDiscriminatedUnionCase>(
                applyToClass,
                applyToClassType,
                partialClass,
                semanticModel,
                applyToClassSymbolInfo,
                cases);            
            var result = generator.Build(context);
            var resultClass = BuildClass(result);            
            var classes = List(new[] { (MemberDeclarationSyntax)resultClass });
            return Task.FromResult(classes);
        }

        private TypeDeclarationSyntax BuildClass(DiscriminatedUnionContext<ClassDiscriminatedUnionCase> context)
        {
            var casesPartialClass = CreateEmptyCasesPartialClass();
            return context.WithMembers(
                List(
                    context.Members.Add(
                        casesPartialClass.WithMembers(
                            List(
                                context.Cases
                                        .Select(c => c.GeneratedPartialClass.WithMembers(List(c.Members)))
                                        .Cast<MemberDeclarationSyntax>()
                            )
                        )
                    )
                )
            )
            .GeneratedPartialClass;
        }

        private TypeDeclarationSyntax BuildStruct(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {
            var casesClass = CreateEmptyCasesPartialClass();
            casesClass = casesClass.WithMembers(List(context.Cases.SelectMany(c => c.Members)));
            var members = context.Members.Add(casesClass);
            return context.WithMembers(List(members)).GeneratedPartialClass;
        }

        private static NameSyntax GetTypeSyntax(TypeDeclarationSyntax c, ISymbol symbol)
        {
            if ((symbol.ContainingNamespace == null || symbol.ContainingNamespace.IsGlobalNamespace) && symbol.Kind == SymbolKind.Namespace)
            {
                return IdentifierName(symbol.Name);
            }
            if (symbol.ContainingNamespace != null && !symbol.ContainingNamespace.IsGlobalNamespace && symbol.Kind == SymbolKind.Namespace)
            {
                return QualifiedName(GetTypeSyntax(c, symbol.ContainingNamespace), IdentifierName(symbol.Name));
            }
            SimpleNameSyntax name;
            if (c.TypeParameterList == null)
            {
                name = IdentifierName(c.Identifier);
            }
            else
            {
                name = GenericName(
                        c.Identifier,
                        TypeArgumentList(
                            SeparatedList<TypeSyntax>(
                                c.TypeParameterList
                                            .ChildNodes()
                                            .Cast<TypeParameterSyntax>()
                                            .Select(p => IdentifierName(p.Identifier))
                            )
                        )
                    );
            }
            if (symbol.ContainingNamespace == null)
            {
                return name;
            }
            var @namespace = GetTypeSyntax(c, symbol.ContainingNamespace);
            return QualifiedName(@namespace, name);
        }

        private static ClassDeclarationSyntax GetPartialClass(
            ClassDeclarationSyntax applyToClass,
            TypeSyntax typeSyntax)
        {
            return ClassDeclaration(applyToClass.Identifier)
                    .AddModifiers(
                        Token(SyntaxKind.AbstractKeyword),
                        Token(SyntaxKind.PartialKeyword)
                        )
                    .AddBaseListTypes(
                        SimpleBaseType(
                            QualifiedName(
                                IdentifierName("System"),
                                GenericName(
                                    Identifier("IEquatable")
                                )
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList(typeSyntax)
                                    )
                                )
                            )
                        )
                    );
        }
                
        private StructDeclarationSyntax GetPartialStruct(StructDeclarationSyntax applyToStruct, TypeSyntax typeSyntax)
        {
            return StructDeclaration(applyToStruct.Identifier)
                   .AddModifiers(
                       Token(SyntaxKind.PartialKeyword)
                       )
                   //.AddBaseListTypes(
                   //    SimpleBaseType(
                   //        QualifiedName(
                   //            IdentifierName("System"),
                   //            GenericName(
                   //                Identifier("IEquatable")
                   //            )
                   //            .WithTypeArgumentList(
                   //                TypeArgumentList(
                   //                    SingletonSeparatedList(typeSyntax)
                   //                )
                   //            )
                   //        )
                   //    )
                   //);
                   ;
        }        

        private static (FieldDeclarationSyntax, SymbolInfo)[] GetCaseParameters(ClassDeclarationSyntax singleCase, SemanticModel semanticModel)
        {
            return singleCase.ChildNodes()
                             .OfType<FieldDeclarationSyntax>()
                             .Where(f => f.Modifiers.Any(m => m.Kind() == SyntaxKind.ReadOnlyKeyword && m.Kind() != SyntaxKind.StaticKeyword))
                             .Select(f => (f, semanticModel.GetSymbolInfo(f.Declaration.Type)))
                             .Select(f =>
                             {
                                 //Debug(f.Item1.Declaration.Variables[0].Identifier.Text);
                                 //Debug(f.Item2.GetType().FullName);
                                 //Debug(f.Item2.Symbol.GetType().FullName);
                                 //SourceTypeParameterSymbol
                                 return f;
                             })
                             .ToArray();
        }

        private ImmutableArray<ClassDiscriminatedUnionCase> GetCases(ClassDeclarationSyntax applyTo, INamedTypeSymbol applyToClassSymbolInfo, SemanticModel semanticModel)
        {
            var casesClass = applyTo.ChildNodes()
                                    .Where(n => n.Kind() == SyntaxKind.ClassDeclaration &&
                                                (n as ClassDeclarationSyntax).Identifier.ValueText == "Cases");

            var cases = casesClass.SelectMany(
                c => c.ChildNodes()
                      .OfType<ClassDeclarationSyntax>()
                      .Where(n => semanticModel.GetDeclaredSymbol(n).BaseType == applyToClassSymbolInfo))
                      .Select((c, i) => new ClassDiscriminatedUnionCase(
                            c,
                            CreateEmptyCasePartialClass(c),
                            GetCaseParameters(c, semanticModel).Select(f => new CaseValue(f.Item1, f.Item2.Symbol as ITypeSymbol)).ToImmutableArray(),
                            i + 1
                            )
                      )
                      .ToImmutableArray();
            return cases;
        }

        private ImmutableArray<StructDiscriminatedUnionCase> GetStructCases(
            StructDeclarationSyntax applyTo,             
            SemanticModel semanticModel)
        {
            var casesClass = applyTo.ChildNodes()
                                    .OfType<ClassDeclarationSyntax>()
                                    .Single(c => c.Identifier.ValueText == "Cases");
            var symbol = semanticModel.GetDeclaredSymbol(casesClass);
            //singleton cases
            var singletonCases = symbol.GetAttributes()
                                       .Select(a => (string)a.ConstructorArguments[0].Value)
                                       .Select((c, i) => new StructDiscriminatedUnionCase(Identifier(c), ImmutableArray<CaseValue>.Empty, i));
            
            return singletonCases.ToImmutableArray();
        }

        private ClassDeclarationSyntax CreateEmptyCasesPartialClass()
        {
            var casesPartialClass =
                ClassDeclaration(Identifier("Cases"))
                             .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PrivateKeyword),
                                    Token(SyntaxKind.StaticKeyword),
                                    Token(SyntaxKind.PartialKeyword)
                                )
                              );
            return casesPartialClass;
        }

        private ClassDeclarationSyntax CreateEmptyCasePartialClass(ClassDeclarationSyntax partialClass)
        {
            var casesPartialClass =
                ClassDeclaration(partialClass.Identifier)
                             .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.PartialKeyword)
                                )
                              );
            return casesPartialClass;
        }

        public static void Debug(string value)
        {
            System.IO.File.AppendAllText(@"C:\test\classes.txt", value + Environment.NewLine);
        }               
    }
}
