using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CSharpDiscriminatedUnion.Generator.Generators.Class
{
    internal sealed class GenerateClassCasesFactoryMethods : GenerateCaseFactoryMethods<DiscriminatedUnionCase>
    {
        public GenerateClassCasesFactoryMethods(string prefix, bool preventNull)
            : base(prefix, preventNull, GenerateSingletonInitializer, GenerateFactoryMethodReturnStatement)
        {
        }

        private static ExpressionSyntax GenerateFactoryMethodReturnStatement(
            DiscriminatedUnionContext<DiscriminatedUnionCase> context, 
            DiscriminatedUnionCase @case)
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
            DiscriminatedUnionContext<DiscriminatedUnionCase> context,
            DiscriminatedUnionCase @case)
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
