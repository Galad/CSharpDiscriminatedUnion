using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    internal sealed class GenerateClassCasesFactoryMethods : GenerateCaseFactoryMethods<ClassDiscriminatedUnionCase>
    {
        public GenerateClassCasesFactoryMethods(string prefix, bool preventNull)
            : base(prefix, preventNull, GenerateSingletonInitializer, GenerateFactoryMethodReturnStatement)
        {
        }

        private static ExpressionSyntax GenerateFactoryMethodReturnStatement(
            DiscriminatedUnionContext<ClassDiscriminatedUnionCase> context, 
            ClassDiscriminatedUnionCase @case)
        {
            return ObjectCreationExpression(
                        QualifiedName(
                            IdentifierName("Cases"),
                            IdentifierName(@case.Name)
                        )
                    )
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList(
                                @case.CaseValues.Select(p => Argument(IdentifierName(p.Name)))
                            )
                        )
                    );
        }

        private static ExpressionSyntax GenerateSingletonInitializer(
            DiscriminatedUnionContext<ClassDiscriminatedUnionCase> context,
            ClassDiscriminatedUnionCase @case)
        {
            return ObjectCreationExpression(
                        QualifiedName(
                            IdentifierName("Cases"),
                            IdentifierName(@case.Name)
                        )
                    )
                    .WithArgumentList(ArgumentList());
        }
    }
}
