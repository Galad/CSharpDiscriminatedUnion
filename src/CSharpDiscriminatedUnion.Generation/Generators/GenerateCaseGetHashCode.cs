using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    internal sealed class GenerateCaseGetHashCode : IDiscriminatedUnionGenerator<ClassDiscriminatedUnionCase>
    {
        private const int PrimeNumber = 16777619;
        private const string PrimeConstant = "prime";
        private const string HashCodeVariable = "hash";
        private static readonly IdentifierNameSyntax HashCodeVariableIdentifier = IdentifierName(HashCodeVariable);

        public DiscriminatedUnionContext<ClassDiscriminatedUnionCase> Build(DiscriminatedUnionContext<ClassDiscriminatedUnionCase> context)
        {
            return context.WithCases(context.Cases.Select(c => c.AddMember(GenerateGetHashCode(c))).ToImmutableArray());
        }

        private MemberDeclarationSyntax GenerateGetHashCode(ClassDiscriminatedUnionCase c)
        {
            return MethodDeclaration(PredefinedType(Token(SyntaxKind.IntKeyword)), "GetHashCode")
                .WithModifiers(
                    TokenList(
                        Token(SyntaxKind.PublicKeyword),
                        Token(SyntaxKind.OverrideKeyword)
                    )
                )
                .WithBody(
                    Block(
                        SingletonList<StatementSyntax>(
                            CheckedStatement(
                                SyntaxKind.UncheckedStatement,
                                Block(GenerateGetHashCodeBody(c))
                            )
                        )
                    )
                );
        }

        private IEnumerable<StatementSyntax> GenerateGetHashCodeBody(ClassDiscriminatedUnionCase @case)
        {
            if (@case.CaseValues.Length == 0)
            {
                yield return ReturnStatement(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(@case.CaseNumber)));
                yield break;
            }

            yield return DeclarePrimeConstant();
            yield return DeclareHashCodeVariable();
            yield return GenerateHashCodeForFieldValue(
                LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    Literal(@case.CaseNumber)
                )
            );
            foreach (var caseValue in @case.CaseValues)
            {
                yield return GenerateHashCodeForFieldValue(HashCodeForCaseValue(caseValue));
            }

            yield return ReturnStatement(HashCodeVariableIdentifier);
        }

        private static StatementSyntax GenerateHashCodeForFieldValue(ExpressionSyntax hashCode)
        {
            return ExpressionStatement(
                AssignmentExpression(
                    SyntaxKind.SimpleAssignmentExpression,
                    HashCodeVariableIdentifier,
                    BinaryExpression(
                        SyntaxKind.ExclusiveOrExpression,
                        ParenthesizedExpression(
                            BinaryExpression(
                                SyntaxKind.MultiplyExpression,
                                HashCodeVariableIdentifier,
                                IdentifierName(PrimeConstant)
                            )
                        ),
                        hashCode
                    )
                )
            );
        }

        private static StatementSyntax DeclareHashCodeVariable()
        {
            return LocalDeclarationStatement(
                    VariableDeclaration(
                        PredefinedType(
                            Token(SyntaxKind.IntKeyword)
                        )
                    )
                    .WithVariables(
                        SingletonSeparatedList(
                            VariableDeclarator(HashCodeVariable)
                            .WithInitializer(
                                EqualsValueClause(
                                    CastExpression(
                                        PredefinedType(
                                            Token(SyntaxKind.IntKeyword)
                                        ),
                                        LiteralExpression(
                                            SyntaxKind.NumericLiteralExpression,
                                            Literal(2166136261)
                                        )
                                    )
                                )
                            )
                        )
                    )
                );                                    
        }

        private static StatementSyntax DeclarePrimeConstant()
        {
            return LocalDeclarationStatement(
                        VariableDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.IntKeyword)
                            )
                        )
                        .WithVariables(
                            SingletonSeparatedList<VariableDeclaratorSyntax>(
                                VariableDeclarator(
                                    Identifier(PrimeConstant)
                                )
                                .WithInitializer(
                                    EqualsValueClause(
                                        LiteralExpression(
                                            SyntaxKind.NumericLiteralExpression,
                                            Literal(PrimeNumber)
                                        )
                                    )
                                )
                            )
                        )
                    )
                    .WithModifiers(
                        TokenList(
                            Token(SyntaxKind.ConstKeyword)
                        )
                    );
        }

        private ExpressionSyntax HashCodeForCaseValue(CaseValue caseValue)
        {
            if (caseValue.SymbolInfo.IsValueType)
            {
                return InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(caseValue.Name),
                        IdentifierName("GetHashCode")
                    )
                );
            }
            return InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ParenthesizedExpression(
                                ConditionalAccessExpression(
                                    IdentifierName(caseValue.Name),
                                    InvocationExpression(
                                        MemberBindingExpression(IdentifierName("GetHashCode"))
                                    )
                                )
                            ),
                            IdentifierName("GetValueOrDefault")
                        )
                    )
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(
                                    PrefixUnaryExpression(
                                        SyntaxKind.UnaryMinusExpression,
                                        LiteralExpression(
                                            SyntaxKind.NumericLiteralExpression,
                                            Literal(1)
                                        )
                                    )
                                )
                            )
                        )
                    );
        }
    }
}
