using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generator.Generators
{
    internal sealed class GenerateBaseGetHashCodeImplementation<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {
            return context.AddMember(
                MethodDeclaration(
                    PredefinedType(
                        Token(SyntaxKind.IntKeyword)
                    ),
                    Identifier("GetHashCode")
                )
                .WithModifiers(
                    TokenList(
                        new[]{
                            Token(SyntaxKind.PublicKeyword),
                            Token(SyntaxKind.OverrideKeyword)
                        }
                    )
                )
                .WithBody(
                    Block(
                        SingletonList<StatementSyntax>(
                            ThrowStatement(
                                ObjectCreationExpression(
                                    QualifiedName(
                                        IdentifierName("System"),
                                        IdentifierName("InvalidOperationException")
                                    )
                                )
                                .WithArgumentList(
                                    ArgumentList(
                                        SingletonSeparatedList<ArgumentSyntax>(
                                            Argument(
                                                LiteralExpression(
                                                    SyntaxKind.StringLiteralExpression,
                                                    Literal("This method should not be called")
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
    }
}
