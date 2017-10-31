using System;
using System.Collections.Generic;
using System.Linq;
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
                                SeparatedList(
                                    new[] { Parameter(Identifier("tag")).WithType(PredefinedType(Token(SyntaxKind.ByteKeyword))) }                                    
                                    .Concat(GetValueParameters(context))
                                )
                            )
                        )
                       .WithBody(Block(GenerateConstructorBody(context)));
            return context.AddMember(constructor);
        }

        private IEnumerable<ParameterSyntax> GetValueParameters(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {            
            return context.Cases.SelectMany(
                @case => @case.CaseValues.Select(
                    @caseValue => Parameter(caseValue.Name).WithType(caseValue.Type)
                    )
                );
        }

        private IEnumerable<StatementSyntax> GenerateConstructorBody(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {
            yield return ExpressionStatement(
                                AssignmentExpression(
                                    SyntaxKind.SimpleAssignmentExpression,
                                    IdentifierName("_tag"),
                                    IdentifierName("tag")
                                )
                            );
            var valueAssignments = context.Cases.SelectMany(
                @case => @case.CaseValues.Select(
                    @caseValue => ExpressionStatement(
                                    AssignmentExpression(
                                        SyntaxKind.SimpleAssignmentExpression,
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            ThisExpression(),
                                            IdentifierName(@caseValue.Name)
                                        ),
                                        IdentifierName(caseValue.Name)
                                    )
                                )
                )
            );
            foreach (var valueAssigment in valueAssignments)
            {
                yield return valueAssigment;
            }
        }
    }
}
