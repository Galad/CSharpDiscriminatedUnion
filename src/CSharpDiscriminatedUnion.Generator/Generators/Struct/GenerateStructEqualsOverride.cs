using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpDiscriminatedUnion.Generator.Generators.Struct
{
    internal sealed class GenerateStructEqualsOverride : IDiscriminatedUnionGenerator<StructDiscriminatedUnionCase>
    {
        public DiscriminatedUnionContext<StructDiscriminatedUnionCase> Build(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {
            return context.AddMember(GenerateEqualsOverride(context));
        }

        private MemberDeclarationSyntax GenerateEqualsOverride(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {
            return GeneratorHelpers.EqualOverrideMethodDeclarationSyntax
                                   .WithBody(Block(GenerateEqualsStatements(context)));
        }

        private StatementSyntax GenerateEqualsStatements(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {
            return ReturnStatement(
                BinaryExpression(
                    SyntaxKind.LogicalAndExpression,
                    BinaryExpression(
                        SyntaxKind.IsExpression,
                        IdentifierName("obj"),
                        context.Type
                    ),
                    InvocationExpression(
                        IdentifierName("Equals"),
                        ArgumentList(
                            SingletonSeparatedList(
                                Argument(
                                    CastExpression(
                                        context.Type,
                                        IdentifierName("obj")                                        
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
