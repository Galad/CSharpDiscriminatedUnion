using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpDiscriminatedUnion.Generation.Generators.Class
{
    internal sealed class GenerateCaseGetHashCode : GenerateGetHashCode<DiscriminatedUnionCase>
    {
        public override DiscriminatedUnionContext<DiscriminatedUnionCase> Build(DiscriminatedUnionContext<DiscriminatedUnionCase> context)
        {
            return context.WithCases(context.Cases.Select(c => c.AddMember(GenerateGetHashCodeMethod(GenerateGetHashCodeBody(c)))).ToImmutableArray());
        }
        
        private IEnumerable<StatementSyntax> GenerateGetHashCodeBody(DiscriminatedUnionCase @case)
        {
            if (@case.CaseValues.Length == 0)
            {
                yield return ReturnStatement(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(@case.CaseNumber)));
                yield break;
            }

            yield return DeclarePrimeConstant();
            yield return DeclareHashCodeVariable();
            yield return GenerateHashCodeForFieldValue(
                LiteralExpression(
                    SyntaxKind.NumericLiteralExpression,
                    Literal(@case.CaseNumber)
                )
            );
            foreach (var caseValue in @case.CaseValues)
            {
                yield return GenerateHashCodeForFieldValue(HashCodeForCaseValue(caseValue));
            }

            yield return ReturnStatement(HashCodeVariableIdentifier);
        }

    }
}
