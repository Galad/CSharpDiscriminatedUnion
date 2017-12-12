using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpDiscriminatedUnion.Generation.Generators.Struct
{
    internal sealed class GenerateStructGetHashCode : GenerateGetHashCode<StructDiscriminatedUnionCase>
    {
        public override DiscriminatedUnionContext<StructDiscriminatedUnionCase> Build(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {
            return context.AddMember(GenerateGetHashCodeMethod(GenerateGetHashCodeBody(context)));
        }

        private IEnumerable<StatementSyntax> GenerateGetHashCodeBody(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {
            yield return DeclarePrimeConstant();
            yield return DeclareHashCodeVariable();
            var tag = MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    ThisExpression(),
                    IdentifierName("_tag")
                );
            yield return GenerateHashCodeForFieldValue(tag);
            var switchCases = context.Cases.Select(c => GetSwitchCase(c));
            yield return SwitchStatement(tag)
                .WithSections(
                    List(switchCases)
                );
            yield return ReturnStatement(IdentifierName(Identifier(HashCodeVariable)));
        }

        private SwitchSectionSyntax GetSwitchCase(StructDiscriminatedUnionCase @case)
        {
            return SwitchSection()
                .WithLabels(
                    SingletonList<SwitchLabelSyntax>(
                        CaseSwitchLabel(
                            LiteralExpression(
                                SyntaxKind.NumericLiteralExpression,
                                Literal(@case.CaseNumber)
                            )
                        )
                    )
                )
                .WithStatements(
                    List<StatementSyntax>(@case.CaseValues.Select(c => GenerateHashCodeForFieldValue(HashCodeForCaseValue(c))))
                        .Add(BreakStatement())
                );
        }
    }
}
