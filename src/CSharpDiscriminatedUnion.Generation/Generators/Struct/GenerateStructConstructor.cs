using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpDiscriminatedUnion.Generation.Generators.Struct
{
    internal class GenerateStructConstructor : IDiscriminatedUnionGenerator<StructDiscriminatedUnionCase>
    {
        public DiscriminatedUnionContext<StructDiscriminatedUnionCase> Build(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {            
            var constructor = ConstructorDeclaration(context.UserDefinedClass.Identifier)
                       .AddModifiers(Token(SyntaxKind.PrivateKeyword))
                       .WithParameterList(
                            ParameterList(                                
                                SingletonSeparatedList(
                                    Parameter(Identifier("tag"))
                                    .WithType(PredefinedType(Token(SyntaxKind.ByteKeyword)))
                                )
                            )
                        )
                       .WithBody(Block(GenerateConstructorBody()));
            return context.AddMember(constructor);
        }

        private IEnumerable<StatementSyntax> GenerateConstructorBody()
        {
            yield return ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("_tag"),
                                    IdentifierName("tag")
                                )
                            );
        }
    }
}
