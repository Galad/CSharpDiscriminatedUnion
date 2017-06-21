using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    internal static class GeneratorHelpers
    {
        public static MethodDeclarationSyntax CreateMatchMethod(ImmutableArray<ClassDiscriminatedUnionCase> cases, SyntaxToken generateParameterName)
        {
            var match =
                MethodDeclaration(IdentifierName(generateParameterName), "Match")
                             .AddTypeParameterListParameters(TypeParameter(generateParameterName))
                             .AddParameterListParameters(cases.Select(c => GetCaseMatchFunction(c, generateParameterName)).ToArray());
            return match;
        }

        private static ParameterSyntax GetCaseMatchFunction(ClassDiscriminatedUnionCase @case, SyntaxToken generateParameterName)
        {
            return Parameter(
                    Identifier("match" + @case.Name.Text)
                    )
                    .WithType(GenericName(
                        Identifier("System.Func"),
                        TypeArgumentList(
                            SeparatedList(
                                @case.CaseValues.Select(vc => vc.Type).Concat(new[] { IdentifierName(generateParameterName) })
                                )
                            )
                        )
                    );
        }

        public static StatementSyntax CreateGuardForNull(IdentifierNameSyntax identifier)
        {
            return IfStatement(
                BinaryExpression(
                        SyntaxKind.EqualsExpression,
                        identifier,
                        LiteralExpression(
                            SyntaxKind.NullLiteralExpression
                        )
                    ),
                Block(
                    SingletonList<StatementSyntax>(
                        ThrowStatement(
                            ObjectCreationExpression(
                                QualifiedName(
                                    IdentifierName("System"),
                                    IdentifierName("ArgumentNullException")
                                )
                            )
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(
                                            InvocationExpression(
                                                IdentifierName("nameof")
                                            )
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList(
                                                        Argument(identifier)
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            );
        }


        public static MethodDeclarationSyntax GenerateEquatableImplementation(TypeSyntax applyToClassType, string parameterName)
        {
            return MethodDeclaration(
                                PredefinedType(Token(SyntaxKind.BoolKeyword)),
                                Identifier("Equals")
                            )
                            .WithParameterList(
                                ParameterList(
                                    SingletonSeparatedList(
                                        Parameter(Identifier(parameterName))
                                            .WithType(applyToClassType)
                                    )
                                )
                            );
        }

        public static InvocationExpressionSyntax InvokeReferenceEquals(ExpressionSyntax left, ExpressionSyntax right)
        {
            return InvocationExpression(IdentifierName("ReferenceEquals"))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    Argument(left),
                                    Token(SyntaxKind.CommaToken),
                                    Argument(right)
                                }
                            )
                        )
                    );
        }

        public static StatementSyntax ReturnFalse()
        {
            return Block(
                SingletonList<StatementSyntax>(
                    ReturnStatement(FalseExpression())
                )
            );
        }

        public static StatementSyntax ReturnTrue()
        {
            return Block(
                SingletonList<StatementSyntax>(
                    ReturnStatement(TrueExpression())
                )
            );
        }

        public static LiteralExpressionSyntax FalseExpression()
        {
            return LiteralExpression(SyntaxKind.FalseLiteralExpression, Token(SyntaxKind.FalseKeyword));
        }

        public static LiteralExpressionSyntax TrueExpression()
        {
            return LiteralExpression(SyntaxKind.TrueLiteralExpression, Token(SyntaxKind.TrueKeyword));
        }

        public static LiteralExpressionSyntax NullExpression()
        {
            return LiteralExpression(SyntaxKind.NullLiteralExpression, Token(SyntaxKind.NullKeyword));
        }

        private static readonly ImmutableArray<string> _matchGenericParametersCandidates = ImmutableArray.Create("T", "TResult", "TMatch", "TMatchResult");
        private const string GeneratedMatchGenericParameter = "TResult";

        public static SyntaxToken GenerateMatchResultGenericParameterName(TypeDeclarationSyntax @class, bool isGeneric)
        {
            if (!isGeneric)
            {
                return Identifier("T");
            }
            var genericParameterNames = @class.TypeParameterList.Parameters.Select(p => p.Identifier.ValueText).ToImmutableHashSet();

            foreach (var candidate in _matchGenericParametersCandidates)
            {
                if (!genericParameterNames.Contains(candidate))
                {
                    return Identifier(candidate);
                }
            }
            int i;
            for (i = 1; !genericParameterNames.Contains(GeneratedMatchGenericParameter + i.ToString()); i++)
            {
            }
            return Identifier(GeneratedMatchGenericParameter + i.ToString());
        }
    }
}
