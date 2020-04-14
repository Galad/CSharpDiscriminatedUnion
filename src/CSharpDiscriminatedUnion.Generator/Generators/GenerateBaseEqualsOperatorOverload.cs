using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using CSharpDiscriminatedUnion.Generator;

namespace CSharpDiscriminatedUnion.Generator.Generators
{
    /// <summary>
    /// Generate the operator override ==
    /// </summary>
    internal sealed class GenerateBaseEqualsOperatorOverload<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {            
            return context.AddMembers(
                new[]
                {
                    GenerateEqualsOperatorOverload(context),
                    GenerateNotEqualsOperatorOverload(context)
                });
        }

        private static MemberDeclarationSyntax GenerateEqualsOperatorOverload(DiscriminatedUnionContext<T> context)
        {
            return DeclareOperator(context, SyntaxKind.EqualsEqualsToken, GenerateEqualsStatements(context is DiscriminatedUnionContext<DiscriminatedUnionCase>));
        }

        private static MemberDeclarationSyntax DeclareOperator(
            DiscriminatedUnionContext<T> context,
            SyntaxKind operatorSyntaxKind,
            IEnumerable<StatementSyntax> body)
        {
            return OperatorDeclaration(
                            PredefinedType(Token(SyntaxKind.BoolKeyword)),
                            Token(operatorSyntaxKind)
                        )
                        .WithParameterList(
                            ParameterList(
                                SeparatedList<ParameterSyntax>(
                                    new[]
                                    {
                            Parameter(Identifier("left")).WithType(context.Type),
                            Parameter(Identifier("right")).WithType(context.Type)
                                    }
                                )
                            )
                        )
                        .WithModifiers(
                            TokenList(
                                Token(SyntaxKind.PublicKeyword),
                                Token(SyntaxKind.StaticKeyword)
                            )
                        )
                        .WithBody(Block(body));
        }

        private static IEnumerable<StatementSyntax> GenerateEqualsStatements(bool isReferenceType)
        {
            if (isReferenceType)
            {
                yield return IfStatement(
                        GeneratorHelpers.InvokeReferenceEquals(IdentifierName("left"), GeneratorHelpers.NullExpression()),
                        Block(
                            ReturnStatement(
                                GeneratorHelpers.InvokeReferenceEquals(IdentifierName("right"), GeneratorHelpers.NullExpression())
                            )
                        )
                    );
            }
            yield return ReturnStatement(
                    InvocationExpression(
                        QualifiedName(IdentifierName("left"), IdentifierName("Equals")),
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(IdentifierName("right"))
                            )
                        )
                    )
                );
        }

        private static MemberDeclarationSyntax GenerateNotEqualsOperatorOverload(DiscriminatedUnionContext<T> context)
        {
            return DeclareOperator(context, SyntaxKind.ExclamationEqualsToken, new[] { GenerateNotEqualsStatements() });
        }

        private static StatementSyntax GenerateNotEqualsStatements()
        {
            return ReturnStatement(
                PrefixUnaryExpression(
                    SyntaxKind.LogicalNotExpression,
                    ParenthesizedExpression(
                        BinaryExpression(
                            SyntaxKind.EqualsExpression,
                            IdentifierName("left"),
                            IdentifierName("right")
                        )
                    )
                )
            );
        }
    }
}
