﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    internal sealed class GenerateDebugView<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {
            return context.AddMember(GenerateDebugViewProperty(context));
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
            return @case.CaseValues
                .Select(c => c.SymbolInfo.IsReferenceType ? c.Name.Text + "String" : c.Name.Text)
                .Select(s => Interpolation(IdentifierName(s)));
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
                                            IdentifierName("variableName"),
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