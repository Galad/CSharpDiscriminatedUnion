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

namespace CSharpDiscriminatedUnion.Generation
{
    public class CodeGenerator : ICodeGenerator
    {
        private static readonly IDiscriminatedUnionGenerator _defaultGenerator = new DefaultDiscriminatedUnionGenerator();
        public CodeGenerator(AttributeData attributeData)
        {
            Requires.NotNull(attributeData, nameof(attributeData));
        }

#if DEBUG
        private static string currentClass;
#endif
        public Task<SyntaxList<MemberDeclarationSyntax>> GenerateAsync(MemberDeclarationSyntax applyTo, CSharpCompilation compilation, IProgress<Diagnostic> progress, CancellationToken cancellationToken)
        {
#if DEBUG
            currentClass = (applyTo as ClassDeclarationSyntax)?.Identifier.ValueText;
#endif
            var semanticModel = compilation.GetSemanticModel(applyTo.SyntaxTree);
            var applyToClass = applyTo as ClassDeclarationSyntax;
            if (applyToClass == null)
            {
                throw new InvalidOperationException("applyTo is not a ClassDeclarationSyntax");
            }

            var applyToClassSymbolInfo = semanticModel.GetDeclaredSymbol(applyTo) as INamedTypeSymbol;
            if (applyToClassSymbolInfo == null)
            {
                throw new InvalidOperationException($"applyTo symbol has not been found");
            }
            var applyToClassType = GetTypeSyntax(applyToClass, applyToClassSymbolInfo);
            var partialClass = GetPartialClass(applyToClass, applyToClassType);
            var cases = GetCases(applyToClass, applyToClassSymbolInfo, semanticModel)
                .Select((c, i) => new DiscriminatedUnionCase(
                    c,
                    CreateEmptyCasePartialClass(c),
                    GetCaseParameters(c, semanticModel).Select(f => new CaseValue(f.Item1, f.Item2.Symbol as ITypeSymbol)).ToImmutableArray(),
                    i + 1
                    )
                )
                .ToImmutableArray();
           
            var context = new DiscriminatedUnionContext(
                applyToClass,
                applyToClassType,
                partialClass,
                semanticModel,
                applyToClassSymbolInfo,
                cases);

            var result = _defaultGenerator.Build(context);

            //*partialClass = AddCasesMembers(applyToClass, partialClass, cases);

            //*casesPartialClass = AddCasesPartialClasses(casesPartialClass, cases);

            //*partialClass = partialClass.AddMembers(casesPartialClass);
            //*partialClass = *AddAbstractMatchMethod(partialClass, cases);
            //partialClass = *AddEqualityImplementations(partialClass, applyToClass);

            //var usingSystem = SyntaxFactory.UsingDirective(SyntaxFactory.IdentifierName("System"));
            //var classes = SyntaxFactory.List(new[] { (MemberDeclarationSyntax)partialClass });
            var resultClass = BuildClass(result);
            var classes = List(new[] { (MemberDeclarationSyntax)resultClass });
            return Task.FromResult(classes);
        }

        private ClassDeclarationSyntax BuildClass(DiscriminatedUnionContext context)
        {
            var casesPartialClass = CreateEmptyCasesPartialClass();
            return context.GeneratedPartialClass.WithMembers(
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
            );
        }

        private static NameSyntax GetTypeSyntax(ClassDeclarationSyntax c, ISymbol symbol)
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

        private IEnumerable<ClassDeclarationSyntax> GetCases(ClassDeclarationSyntax applyTo, INamedTypeSymbol applyToClassSymbolInfo, SemanticModel semanticModel)
        {
            var casesClass = applyTo.ChildNodes()
                                    .Where(n => n.Kind() == SyntaxKind.ClassDeclaration &&
                                                (n as ClassDeclarationSyntax).Identifier.ValueText == "Cases");

            var cases = casesClass.SelectMany(
                c => c.ChildNodes()
                      .OfType<ClassDeclarationSyntax>()
                      .Where(n => semanticModel.GetDeclaredSymbol(n).BaseType == applyToClassSymbolInfo));        
            return cases;
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

        //private ClassDeclarationSyntax AddAbstractMatchMethod(
        //    ClassDeclarationSyntax partialClass,
        //    ClassDeclarationSyntax[] cases)
        //{
        //    var match =
        //        SyntaxFactory.MethodDeclaration(SyntaxFactory.IdentifierName("T"), "Match")
        //                     .AddTypeParameterListParameters(SyntaxFactory.TypeParameter(SyntaxFactory.Identifier("T")))
        //                     .AddParameterListParameters(cases.Select(GetCaseMatchFunction).ToArray())
        //                     .WithModifiers(
        //                        SyntaxFactory.TokenList(
        //                            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
        //                            SyntaxFactory.Token(SyntaxKind.AbstractKeyword)
        //                        )
        //                     )
        //                     .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
        //    return partialClass.AddMembers(match);
        //}

        //private ParameterSyntax GetCaseMatchFunction(ClassDeclarationSyntax caseClass)
        //{
        //    return SyntaxFactory.Parameter(
        //            SyntaxFactory.Identifier("match" + caseClass.Identifier.Text)
        //        ).WithType(SyntaxFactory.GenericName(
        //            SyntaxFactory.Identifier("System.Func"),
        //            SyntaxFactory.TypeArgumentList(
        //                SyntaxFactory.SeparatedList(
        //                    GetCaseValues(caseClass).Concat(new[] { SyntaxFactory.IdentifierName("T") })
        //                    )
        //                )
        //            )
        //        );
        //}

        //private IEnumerable<TypeSyntax> GetCaseValues(ClassDeclarationSyntax caseClass)
        //{
        //    var caseParameters = GetCaseParameters(caseClass);
        //    return caseParameters.Select(f => f.Declaration.Type);
        //}

        //private ClassDeclarationSyntax AddEqualityImplementations(
        //    ClassDeclarationSyntax partialClass,
        //    ClassDeclarationSyntax applyToClass)
        //{
        //    var applyToClassType = GetTypeSyntax(applyToClass);
        //    var equatableImplementation =
        //        MethodDeclaration(
        //            PredefinedType(Token(SyntaxKind.BoolKeyword)),
        //            Identifier("Equals")
        //        )
        //        .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
        //        .WithParameterList(
        //            ParameterList(
        //                SingletonSeparatedList(
        //                    Parameter(Identifier("value"))
        //                                 .WithType(applyToClassType)
        //                )
        //            )
        //        )
        //        .WithBody(Block(GetEquatableBody()));
        //    return partialClass.AddMembers(equatableImplementation);
        //}

        //private SyntaxList<StatementSyntax> GetEquatableBody()
        //{
        //    return SingletonList<StatementSyntax>(
        //                    ReturnStatement(
        //                        LiteralExpression(
        //                            SyntaxKind.FalseLiteralExpression
        //                        )
        //                    )
        //                );
        //}

        //private ClassDeclarationSyntax AddCasesPartialClasses(ClassDeclarationSyntax partialClass, ClassDeclarationSyntax[] cases)
        //{
        //    return partialClass.AddMembers(
        //        cases.Select(c => CreateCasePartialClassConstructor(CreateEmptyCasePartialClass(c), c)).ToArray()
        //        );
        //}

        //private ClassDeclarationSyntax CreateCasePartialClassConstructor(ClassDeclarationSyntax partialClass, ClassDeclarationSyntax parametersContainer)
        //{
        //    var caseParameters = GetCaseParameters(parametersContainer);            
        //    var constructor =
        //         SyntaxFactory.ConstructorDeclaration(partialClass.Identifier)
        //                      .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
        //                      .AddParameterListParameters(
        //                          caseParameters.Select(f => SyntaxFactory.Parameter(f.Declaration.Variables[0].Identifier)
        //                                                                  .WithType(f.Declaration.Type))
        //                                        .ToArray()
        //                       )
        //                      .WithBody(
        //                        SyntaxFactory.Block(
        //                            caseParameters.Select(f =>
        //                                SyntaxFactory.ExpressionStatement(
        //                                    SyntaxFactory.AssignmentExpression(
        //                                        SyntaxKind.SimpleAssignmentExpression,
        //                                        SyntaxFactory.MemberAccessExpression(
        //                                            SyntaxKind.SimpleMemberAccessExpression,
        //                                            SyntaxFactory.ThisExpression(),
        //                                            SyntaxFactory.IdentifierName(f.Declaration.Variables[0].Identifier)),
        //                                        SyntaxFactory.IdentifierName(f.Declaration.Variables[0].Identifier)
        //                                    )
        //                                )
        //                            )
        //                        )
        //                      );
        //    return partialClass.AddMembers(constructor);
        //}

        //private ClassDeclarationSyntax AddCasesMembers(
        //    ClassDeclarationSyntax applyTo,
        //    ClassDeclarationSyntax partialClass,
        //    IEnumerable<SyntaxNode> cases)
        //{
        //    var singleCase = cases.SingleOrDefault() as ClassDeclarationSyntax;
        //    if (singleCase == null)
        //    {
        //        return partialClass;
        //    }

        //    var caseParameters = GetCaseParameters(singleCase);
        //    MemberDeclarationSyntax member;
        //    if (caseParameters.Length == 0)
        //    {
        //        member = CreateParameterlessCaseMember(applyTo, singleCase);
        //    }
        //    else
        //    {
        //        member = CreateCaseMemberWithParameters(applyTo, singleCase, caseParameters);
        //    }
        //    return partialClass.AddMembers(member);
        //}

        //private MemberDeclarationSyntax CreateCaseMemberWithParameters(ClassDeclarationSyntax applyTo, ClassDeclarationSyntax singleCase, FieldDeclarationSyntax[] caseParameters)
        //{
        //    return SyntaxFactory.MethodDeclaration(
        //                    SyntaxFactory.IdentifierName(applyTo.Identifier),
        //                    singleCase.Identifier)
        //                .WithModifiers(
        //                    SyntaxFactory.TokenList(
        //                        SyntaxFactory.Token(SyntaxKind.PublicKeyword),
        //                        SyntaxFactory.Token(SyntaxKind.StaticKeyword)
        //                    )
        //                )
        //                .WithParameterList(
        //                    SyntaxFactory.ParameterList(
        //                        SyntaxFactory.SeparatedList(
        //                            caseParameters.Select(p =>
        //                                SyntaxFactory.Parameter(p.Declaration.Variables[0].Identifier)
        //                                             .WithType(p.Declaration.Type)
        //                            )
        //                        )
        //                    )
        //                )
        //                .WithBody(
        //                    SyntaxFactory.Block(
        //                        SyntaxFactory.ReturnStatement(
        //                            SyntaxFactory.ObjectCreationExpression(
        //                                SyntaxFactory.QualifiedName(
        //                                    SyntaxFactory.IdentifierName("Cases"),
        //                                    SyntaxFactory.IdentifierName(singleCase.Identifier)
        //                                )
        //                            )
        //                            .WithArgumentList(
        //                                SyntaxFactory.ArgumentList(
        //                                    SyntaxFactory.SeparatedList(
        //                                        caseParameters.Select(p =>
        //                                            SyntaxFactory.Argument(SyntaxFactory.IdentifierName(p.Declaration.Variables[0].Identifier))
        //                                        )
        //                                    )
        //                                )
        //                            )
        //                        )
        //                    )
        //                )
        //                ;
        //}

        //private static MemberDeclarationSyntax CreateParameterlessCaseMember(ClassDeclarationSyntax applyTo, ClassDeclarationSyntax singleCase)
        //{
        //    return SyntaxFactory.FieldDeclaration(
        //                        SyntaxFactory.List<AttributeListSyntax>(),
        //                        SyntaxFactory.TokenList(
        //                            SyntaxFactory.Token(SyntaxKind.PublicKeyword),
        //                            Token(SyntaxKind.StaticKeyword),
        //                            SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)
        //                        ),
        //                        SyntaxFactory.VariableDeclaration(
        //                            SyntaxFactory.IdentifierName(applyTo.Identifier),
        //                            SyntaxFactory.SeparatedList(new[] {
        //                SyntaxFactory.VariableDeclarator(singleCase.Identifier)
        //                             .WithInitializer(
        //                                 SyntaxFactory.EqualsValueClause(
        //                                     SyntaxFactory.ObjectCreationExpression(
        //                                        SyntaxFactory.QualifiedName(
        //                                            SyntaxFactory.IdentifierName("Cases"),
        //                                            SyntaxFactory.IdentifierName(singleCase.Identifier)
        //                                        )
        //                                     )
        //                                     .WithArgumentList(SyntaxFactory.ArgumentList())
        //                                 )
        //                             )
        //                            })
        //                        )
        //                    );
        //}

    }
}
