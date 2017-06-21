using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    /// <summary>
    /// Generates the override of <see cref="Object.Equals(object)"/>
    /// </summary>
    internal sealed class GenerateBaseEqualsOverride<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {
            return context.AddMember(GenerateEqualsOverride(context));
        }

        private static MemberDeclarationSyntax GenerateEqualsOverride(DiscriminatedUnionContext<T> context)
        {
            return MethodDeclaration(
                PredefinedType(Token(SyntaxKind.BoolKeyword)),
                "Equals"
            )
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList(
                        Parameter(Identifier("obj"))
                        .WithType(
                            PredefinedType(
                                Token(SyntaxKind.ObjectKeyword)
                            )
                        )
                    )
                )
            )
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.OverrideKeyword)
                )
            )
            .WithBody(Block(GenerateEqualsStatements(context)));
        }

        private static StatementSyntax GenerateEqualsStatements(DiscriminatedUnionContext<T> context)
        {
            return ReturnStatement(
                    InvocationExpression(
                        IdentifierName("Equals"),                        
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(
                                    BinaryExpression(
                                        SyntaxKind.AsExpression,
                                        IdentifierName("obj"),
                                        context.Type
                                    )
                                )
                            )
                        )
                    )
                );
        }
    }
}
