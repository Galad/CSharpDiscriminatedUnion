using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.Collections.Immutable;

namespace CSharpDiscriminatedUnion.Generation.Generators.Class
{
    /// <summary>
    /// Generates the implementation of <see cref="IEquatable{T}.Equals(T)"/> in the case classes
    /// </summary>
    internal sealed class GenerateCaseEquatableImplementation : IDiscriminatedUnionGenerator<DiscriminatedUnionCase>
    {
        private const string ParameterName = "obj";

        public DiscriminatedUnionContext<DiscriminatedUnionCase> Build(DiscriminatedUnionContext<DiscriminatedUnionCase> context)
        {
            return context.WithCases(
                context.Cases.Select(c => c.AddMember(GenerateEqualsImplementation(context.Type, c, context.Cases))).ToImmutableArray()
                );
        }

        private MethodDeclarationSyntax GenerateEqualsImplementation(TypeSyntax name, DiscriminatedUnionCase @case, ImmutableArray<DiscriminatedUnionCase> allCases)
        {
            return GeneratorHelpers.GenerateEquatableImplementation(name, ParameterName)
                                                     .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword)))
                                                     .WithBody(Block(GenerateEqualsBlock(@case, allCases)));
        }

        private IEnumerable<StatementSyntax> GenerateEqualsBlock(DiscriminatedUnionCase @case, ImmutableArray<DiscriminatedUnionCase> allCases)
        {
            yield return IfValueIsNullThenReturnFalse();
            yield return IfValueIsThisThenReturnTrue();
            yield return ReturnByMatching(@case, allCases);
        }

        private StatementSyntax IfValueIsNullThenReturnFalse()
        {
            return IfStatement(
                GeneratorHelpers.InvokeReferenceEquals(IdentifierName(ParameterName), GeneratorHelpers.NullExpression()),
                GeneratorHelpers.ReturnFalse()
                );
        }

        private StatementSyntax IfValueIsThisThenReturnTrue()
        {
            return IfStatement(
                GeneratorHelpers.InvokeReferenceEquals(IdentifierName(ParameterName), ThisExpression()),
                GeneratorHelpers.ReturnTrue()
                );
        }

        private StatementSyntax ReturnByMatching(DiscriminatedUnionCase @case, ImmutableArray<DiscriminatedUnionCase> allCases)
        {
            return ReturnStatement(
                InvocationExpression(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(ParameterName),
                        IdentifierName("Match")
                    )
                )
                .WithArgumentList(
                    ArgumentList(
                        SeparatedList(
                            allCases.Select(c => c.Name != @case.Name ? LambdaReturnsFalse(c) : LambdaReturnsEquality(c))
                        )
                    )
                )
            );
        }

        private ArgumentSyntax LambdaReturnsFalse(DiscriminatedUnionCase @case)
        {
            var lambdaBody = GeneratorHelpers.FalseExpression();
            return LambdaForCase(@case, lambdaBody);
        }

        private ArgumentSyntax LambdaForCase(DiscriminatedUnionCase @case, ExpressionSyntax lambdaBody)
        {
            if (@case.CaseValues.Length == 1)
            {
                return Argument(SimpleLambdaExpression(Parameter(@case.CaseValues[0].Name), lambdaBody));
            }
            return Argument(
                ParenthesizedLambdaExpression(lambdaBody)
                .WithParameterList(
                    ParameterList(
                        SeparatedList(
                            @case.CaseValues.Select(c => Parameter(c.Name))
                        )
                    )
                )
            );
        }

        private ArgumentSyntax LambdaReturnsEquality(DiscriminatedUnionCase @case)
        {
            if (@case.CaseValues.Length == 0)
            {
                return Argument(ParenthesizedLambdaExpression(GeneratorHelpers.FalseExpression()));
            }
            var lambdaBody =
                @case.CaseValues.Skip(1).Aggregate(
                    EqualityForCase(@case.CaseValues[0]),
                    (exp, c) =>
                    BinaryExpression(
                        SyntaxKind.LogicalAndExpression,
                        exp,
                        EqualityForCase(c)
                    )
                );
            return LambdaForCase(@case, lambdaBody);
        }

        private ExpressionSyntax EqualityForCase(CaseValue caseValue)
        {
            return
                InvocationExpression(
                   MemberAccessExpression(
                       SyntaxKind.SimpleMemberAccessExpression,
                       MemberAccessExpression(
                           SyntaxKind.SimpleMemberAccessExpression,
                           QualifiedName(
                               QualifiedName(
                                   QualifiedName(
                                       IdentifierName("System"),
                                       IdentifierName("Collections")
                                    ),
                                   IdentifierName("Generic")
                                ),
                                GenericName(Identifier("EqualityComparer"))
                                .WithTypeArgumentList(
                                    TypeArgumentList(
                                        SingletonSeparatedList<TypeSyntax>(caseValue.Type)
                                    )
                                )
                            ),
                           IdentifierName("Default")
                        ),
                        IdentifierName("Equals")
                   )
                )
            .WithArgumentList(
                ArgumentList(
                    SeparatedList<ArgumentSyntax>(
                        new SyntaxNodeOrToken[]{
                            Argument(
                                MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    ThisExpression(),
                                    IdentifierName(caseValue.Name)
                                )
                            ),
                            Token(SyntaxKind.CommaToken),
                            Argument(IdentifierName(caseValue.Name))
                        }
                    )
                )
            );
        }
    }
}
