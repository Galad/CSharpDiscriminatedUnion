using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    internal class AddGeneratedCodeAttribute<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        private readonly string _version;
        private readonly string _toolName;

        public AddGeneratedCodeAttribute(string toolName, string version)
        {
            _toolName = toolName;
            _version = version;
        }

        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {
            return context.AddAttributeLists(
                            AttributeList(
                                SingletonSeparatedList(
                                    Attribute(
                                        QualifiedName(
                                            QualifiedName(
                                                QualifiedName(
                                                    IdentifierName("System"),
                                                    IdentifierName("CodeDom")
                                                ),
                                                IdentifierName("Compiler")
                                            ),
                                            IdentifierName("GeneratedCode")
                                        )
                                    )
                                    .WithArgumentList(
                                        AttributeArgumentList(
                                            SeparatedList<AttributeArgumentSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    AttributeArgument(
                                                        LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            Literal(_toolName)
                                                        )
                                                    ),
                                                    Token(SyntaxKind.CommaToken),
                                                    AttributeArgument(
                                                        LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            Literal(_version)
                                                        )
                                                    )
                                                }
                                            )
                                        )
                                    )
                                )
                            )
                        );
        }
    }
}
