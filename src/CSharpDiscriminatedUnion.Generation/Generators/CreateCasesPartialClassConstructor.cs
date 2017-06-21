using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    /// <summary>
    /// Creates the constructor for the partial classes
    /// </summary>
    internal sealed class CreateCasesPartialClassConstructor : IDiscriminatedUnionGenerator<ClassDiscriminatedUnionCase>
    {
        public DiscriminatedUnionContext<ClassDiscriminatedUnionCase> Build(DiscriminatedUnionContext<ClassDiscriminatedUnionCase> context)
        {            
            var cases = context.Cases
                               .Select(c => c.AddMember(CreateCasePartialClassConstructor(c)))
                               .ToImmutableArray();            
            return context.WithCases(cases);
        }

        private MemberDeclarationSyntax CreateCasePartialClassConstructor(ClassDiscriminatedUnionCase unionCase)
        {
            var constructor =
                 SyntaxFactory.ConstructorDeclaration(unionCase.UserDefinedClass.Identifier)
                              .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                              .AddParameterListParameters(
                                  unionCase.CaseValues.Select(v => SyntaxFactory.Parameter(v.Name)
                                                                                .WithType(v.Type))
                                                .ToArray()
                               )
                              .WithBody(
                                SyntaxFactory.Block(
                                    unionCase.CaseValues.Select(v =>
                                        SyntaxFactory.ExpressionStatement(
                                            SyntaxFactory.AssignmentExpression(
                                                SyntaxKind.SimpleAssignmentExpression,
                                                SyntaxFactory.MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    SyntaxFactory.ThisExpression(),
                                                    SyntaxFactory.IdentifierName(v.Name)),
                                                SyntaxFactory.IdentifierName(v.Name)
                                            )
                                        )
                                    )
                                )
                              );
            return constructor;
        }
    }
}
