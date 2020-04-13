using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    /// <summary>
    /// Build the factory methods for each case
    /// </summary>
    internal class GenerateCaseFactoryMethods<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        private readonly string _prefix;
        private readonly bool _preventNull;
        private readonly Func<DiscriminatedUnionContext<T>, T, ExpressionSyntax> _singletonInitializer;
        private readonly Func<DiscriminatedUnionContext<T>, T, ExpressionSyntax> _generateFactoryMethodReturnStatement;

        public GenerateCaseFactoryMethods(
            string prefix,
            bool preventNull,
            Func<DiscriminatedUnionContext<T>, T, ExpressionSyntax> singletonInitializer,
            Func<DiscriminatedUnionContext<T>, T, ExpressionSyntax> generateFactoryMethodReturnStatement)
        {
            _prefix = prefix ?? throw new ArgumentNullException(nameof(prefix));
            _preventNull = preventNull;
            _singletonInitializer = singletonInitializer;
            _generateFactoryMethodReturnStatement = generateFactoryMethodReturnStatement;
        }

        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {
            return context.AddMembers(AddCasesMembers(context));
        }

        private IEnumerable<MemberDeclarationSyntax> AddCasesMembers(DiscriminatedUnionContext<T> context)
        {
            return context.Cases.Select(c =>
            {
                MemberDeclarationSyntax member;
                if (c.CaseValues.Length == 0)
                {
                    member = CreateSingleCaseSingleton(context, c);
                }
                else
                {
                    member = CreateCaseFactoryMethod(context, c);
                }
                return member;
            });
        }

        private MemberDeclarationSyntax CreateSingleCaseSingleton(
            DiscriminatedUnionContext<T> context,
            T singleCase)
        {
            var summaryLines = GetSummaryLines(singleCase);

            return FieldDeclaration(
                        List<AttributeListSyntax>(),
                        TokenList(
                            Token(GeneratorHelpers.CreateXmlDocumentation(summaryLines), SyntaxKind.PublicKeyword, TriviaList()),                            
                            Token(SyntaxKind.StaticKeyword),
                            Token(SyntaxKind.ReadOnlyKeyword)
                        ),
                        VariableDeclaration(
                            context.Type,
                            SeparatedList(new[] {
                                VariableDeclarator(singleCase.Name)
                                .WithInitializer(
                                    EqualsValueClause(
                                        _singletonInitializer(context, singleCase)
                                    )
                                )
                            })
                        )
                    );
        }

        private MemberDeclarationSyntax CreateCaseFactoryMethod(
            DiscriminatedUnionContext<T> context,
            T singleCase)
        {
            var summaryLines = GetSummaryLines(singleCase);
            var parametersDescriptions = singleCase.CaseValues.Select(c => (c.Name.ValueText, c.Description));

            return MethodDeclaration(
                            context.Type,
                            _prefix + singleCase.Name)
                        .WithModifiers(
                            TokenList(
                                Token(GeneratorHelpers.CreateXmlDocumentation(summaryLines, parametersDescriptions), SyntaxKind.PublicKeyword, TriviaList()),
                                Token(SyntaxKind.StaticKeyword)
                            )
                        )
                        .WithParameterList(
                            ParameterList(
                                SeparatedList(
                                    singleCase.CaseValues.Select(p =>
                                        Parameter(p.Name).WithType(p.Type)
                                    )
                                )
                            )
                        )
                        .WithBody(
                            Block(
                                GenerateCreateCaseFactoryBlock(context, singleCase)
                            )
                        )
                        ;
        }

        private static List<string> GetSummaryLines(T singleCase)
        {
            var summaryLines = new List<string>() { "Creates a " + singleCase.Name };
            if (singleCase.Description != null)
            {
                summaryLines.AddRange(singleCase.Description.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries));
            }

            return summaryLines;
        }

        private IEnumerable<StatementSyntax> GenerateCreateCaseFactoryBlock(
            DiscriminatedUnionContext<T> context,
            T singleCase)
        {
            if (_preventNull)
            {
                foreach (var caseValue in singleCase.CaseValues.Where(c => CanHaveNullGuard(c)))
                {
                    var c = caseValue;
                    yield return GeneratorHelpers.CreateGuardForNull(IdentifierName(c.Name));
                }
            }
            yield return ReturnStatement(_generateFactoryMethodReturnStatement(context, singleCase));
        }

        private static bool CanHaveNullGuard(CaseValue c)
        {
            return c.SymbolInfo.IsReferenceType ||
                   c.SymbolInfo.TypeKind == TypeKind.TypeParameter &&
                   !c.SymbolInfo.IsValueType;
        }
    }
}
