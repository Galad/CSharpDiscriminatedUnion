using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpDiscriminatedUnion.Generation.Generators
{

    internal sealed class GenerateDebugView<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {
            if (context.Cases.IsEmpty)
            {
                return context;
            }
            return context.AddMember(GenerateDebugViewProperty(context))
                          .AddAttributeLists(GeneratorHelpers.CreateDebuggerDisplayAttributeList());
        }

        private MemberDeclarationSyntax GenerateDebugViewProperty(DiscriminatedUnionContext<T> context)
        {
            return PropertyDeclaration(
                PredefinedType(Token(SyntaxKind.StringKeyword)),
                Identifier("DebugView")
                )
                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
                .WithAccessorList(
                    AccessorList(
                        SingletonList(
                            AccessorDeclaration(
                                SyntaxKind.GetAccessorDeclaration
                            )
                            .WithBody(
                                Block(
                                    SingletonList<StatementSyntax>(
                                        ReturnStatement(GeneratePropertyExpression(context))
                                    )
                                )
                            )
                        )
                    )
                );
        }

        private static ExpressionSyntax GeneratePropertyExpression(DiscriminatedUnionContext<T> context)
        {
            return InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        ThisExpression(),
                        IdentifierName("Match")
                    )
                )
                .WithArgumentList(ArgumentList(SeparatedList(context.Cases.Select(c => Argument(GenerateCaseFunc(c))))));
        }

        private static ExpressionSyntax GenerateCaseFunc(T @case)
        {
            var referenceTypeToStringStatements =
                @case.CaseValues
                     .Where(c => c.SymbolInfo.IsReferenceType)
                     .Select(c => GenerateReferenceTypeToStringStatement(c));
            var returnStatement =
                ReturnStatement(
                    InterpolatedStringExpression(
                        Token(SyntaxKind.InterpolatedStringStartToken)
                    )
                    .WithContents(
                        List<InterpolatedStringContentSyntax>(
                            new[]{
                                InterpolatedStringText()
                                .WithTextToken(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.InterpolatedStringTextToken,
                                        $"{@case.Name.Text}(",
                                        $"{@case.Name.Text}(",
                                        TriviaList()
                                    )
                                )
                            }
                            .Concat(GenerateInterpolatedStrings(@case))
                            .Concat(new[]{InterpolatedStringText()
                                .WithTextToken(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.InterpolatedStringTextToken,
                                        ")",
                                        ")",
                                        TriviaList()
                                    )
                                )
                            })
                            .ToArray()
                        )
                    )
                );
            return ParenthesizedLambdaExpression(
                ParameterList(
                    SeparatedList(
                        @case.CaseValues.Select(c => Parameter(c.Name))
                    )
                ),
                Block(
                    referenceTypeToStringStatements.Concat(new[] { returnStatement })
                )
            );
        }

        private static IEnumerable<InterpolatedStringContentSyntax> GenerateInterpolatedStrings(T @case)
        {
            if (@case.CaseValues.IsEmpty)
            {
                return Enumerable.Empty<InterpolatedStringContentSyntax>();
            }
            var interpolatedStrings = @case.CaseValues
                .Select(c => c.SymbolInfo.IsReferenceType ? c.Name.Text + "String" : c.Name.Text)
                .Select(s => (InterpolatedStringContentSyntax)Interpolation(IdentifierName(s)));
            if(@case.CaseValues.Length == 1)
            {
                return interpolatedStrings;
            }
            var interpolatedStringsAndSeparator =
                interpolatedStrings.Skip(1)
                                   .SelectMany(s => new[] {
                                       InterpolatedStringText()
                                            .WithTextToken(
                                                Token(
                                                    TriviaList(),
                                                    SyntaxKind.InterpolatedStringTextToken,
                                                    ", ",
                                                    ", ",
                                                    TriviaList()
                                                )
                                            ),
                                       s});
            return interpolatedStrings.Take(1)
                                      .Concat(interpolatedStringsAndSeparator);
        }

        private static StatementSyntax GenerateReferenceTypeToStringStatement(CaseValue c)
        {
            var variableName = c.Name.Text + "String";
            return LocalDeclarationStatement(
                VariableDeclaration(
                    IdentifierName("var")
                )
                .WithVariables(
                    SingletonSeparatedList<VariableDeclaratorSyntax>(
                        VariableDeclarator(
                            Identifier(variableName)
                        )
                        .WithInitializer(
                            EqualsValueClause(
                                ConditionalExpression(
                                    BinaryExpression(
                                        SyntaxKind.EqualsExpression,
                                        IdentifierName(c.Name),
                                        LiteralExpression(
                                            SyntaxKind.NullLiteralExpression
                                        )
                                    ),
                                    LiteralExpression(
                                        SyntaxKind.StringLiteralExpression,
                                        Literal("null")
                                    ),
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName(c.Name),
                                            IdentifierName("ToString")
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            );
        }
    }
}
