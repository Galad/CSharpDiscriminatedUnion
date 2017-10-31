using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    internal abstract class GenerateGetHashCode<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        private const int PrimeNumber = 16777619;
        private const string PrimeConstant = "prime";
        protected const string HashCodeVariable = "hash";
        protected static readonly IdentifierNameSyntax HashCodeVariableIdentifier = IdentifierName(HashCodeVariable);

        public abstract DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context);

        protected MemberDeclarationSyntax GenerateGetHashCodeMethod(IEnumerable<StatementSyntax> statements)
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
                                Block(statements)
                            )
                        )
                    )
                );
        }
      
        protected static StatementSyntax GenerateHashCodeForFieldValue(ExpressionSyntax hashCode)
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

        protected static StatementSyntax DeclareHashCodeVariable()
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

        protected static StatementSyntax DeclarePrimeConstant()
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

        protected ExpressionSyntax HashCodeForCaseValue(CaseValue caseValue)
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
