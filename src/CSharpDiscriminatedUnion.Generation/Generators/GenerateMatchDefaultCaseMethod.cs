using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Linq;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    /// <summary>
    /// Generates the Match method with a default handler
    /// </summary>
    internal class GenerateMatchDefaultCaseMethod<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {
            if (context.Cases.IsEmpty)
            {
                return context;
            }
            return context.AddMember(GetMatchMethod(context));
        }

        private static MemberDeclarationSyntax GetMatchMethod(DiscriminatedUnionContext<T> context)
        {
            var match = GeneratorHelpers.CreateMatchDefaultCaseMethod(context.Cases.Cast<IDiscriminatedUnionCase>(), context.MatchGenericParameter)
                             .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PublicKeyword)
                                )
                             )                             
                             .WithBody(Block(GenerateBody(context)));
            return match;
        }

        private static IEnumerable<StatementSyntax> GenerateBody(DiscriminatedUnionContext<T> context)
        {
            yield return GeneratorHelpers.CreateGuardForNull(IdentifierName("none"));

            var callMatch = InvocationExpression(IdentifierName("Match"))
                        .WithArgumentList(
                            ArgumentList(
                                SeparatedList(context.Cases.Select(c => Argument(GenerateMatchInvocation(c))))
                            )
                        );
            yield return ReturnStatement(callMatch);
        }

        private static ParenthesizedLambdaExpressionSyntax GenerateMatchInvocation(T @case)
        {
            var matchName = "match" + @case.Name;
            return ParenthesizedLambdaExpression(
                        ConditionalExpression(
                            BinaryExpression(
                                SyntaxKind.EqualsExpression,
                                IdentifierName(matchName),
                                LiteralExpression(
                                    SyntaxKind.NullLiteralExpression
                                )
                            ),
                            InvocationExpression(
                                IdentifierName("none")
                            ),
                            InvocationExpression(
                                IdentifierName(matchName)
                            )
                            .WithArgumentList(
                                ArgumentList(
                                    SeparatedList(
                                        @case.CaseValues.Select(cv => Argument(IdentifierName(cv.Name)))
                                    )
                                )
                            )
                        )
                    )
                    .WithParameterList(
                        ParameterList(
                            SeparatedList(
                                @case.CaseValues.Select(cv => Parameter(cv.Name))
                            )
                        )
                    );
        }
    }
}
