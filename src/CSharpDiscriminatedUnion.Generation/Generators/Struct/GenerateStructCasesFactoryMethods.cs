using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpDiscriminatedUnion.Generation.Generators.Struct
{
    internal sealed class GenerateStructCasesFactoryMethods : GenerateCaseFactoryMethods<StructDiscriminatedUnionCase>
    {
        public GenerateStructCasesFactoryMethods(string prefix, bool preventNull)
            : base(prefix, preventNull, GenerateSingletonInitializer, GenerateFactoryMethodReturnStatement)
        {
        }
        
        private static ExpressionSyntax GenerateSingletonInitializer(
            DiscriminatedUnionContext<StructDiscriminatedUnionCase> context,
            StructDiscriminatedUnionCase @case)
        {
            return ObjectCreationExpression(context.Type)
                    .WithArgumentList(
                        ArgumentList(
                            SingletonSeparatedList(
                                CreateTagArgument(@case)
                            )
                        )
                    );
        }

        private static ArgumentSyntax CreateTagArgument(StructDiscriminatedUnionCase @case)
        {
            return Argument(
                        LiteralExpression(
                            Microsoft.CodeAnalysis.CSharp.SyntaxKind.NumericLiteralExpression,
                            Literal(@case.CaseNumber)
                        )
                    );
        }

        private static ExpressionSyntax GenerateFactoryMethodReturnStatement(
            DiscriminatedUnionContext<StructDiscriminatedUnionCase> context,
            StructDiscriminatedUnionCase @case)
        {
            return ObjectCreationExpression(context.Type)
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList(
                                new[] { CreateTagArgument(@case)}.Concat(
                                    context.Cases.SelectMany(c =>  c.CaseValues.Select(p => Argument(IdentifierName(p.Name))))
                                )
                            )
                        )
                    );
        }
    }
}
