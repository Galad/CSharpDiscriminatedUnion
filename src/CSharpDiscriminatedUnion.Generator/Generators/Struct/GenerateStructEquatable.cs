using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpDiscriminatedUnion.Generator.Generators.Struct
{
    internal sealed class GenerateStructEquatable : IDiscriminatedUnionGenerator<StructDiscriminatedUnionCase>
    {
        private const string ParameterName = "value";

        public DiscriminatedUnionContext<StructDiscriminatedUnionCase> Build(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {
            var block = Block(GenerateEquatableBlock(context));
            var method = GeneratorHelpers.GenerateEquatableImplementation(context.Type, ParameterName)
                                         .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword)))
                                         .WithBody(block);
            return context.AddMember(method);
        }

        private IEnumerable<StatementSyntax> GenerateEquatableBlock(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {
            if (!context.IsSingleCase)
            {
                yield return GeneratorHelpers.GenerateStructMatchingSwitchStatement(
                    context.Cases.Cast<IDiscriminatedUnionCase>(),
                    d => GenerateReturnStatement(d, context.SemanticModel));
            }
            else if (context.Cases.IsEmpty)
            {
                yield return ReturnStatement(GeneratorHelpers.TrueExpression());
            }
            else
            {
                yield return GenerateReturnStatementForSingleCase(context.Cases[0], context.SemanticModel);
            }
        }

        private ReturnStatementSyntax GenerateReturnStatement(IDiscriminatedUnionCase @case, SemanticModel semanticModel)
        {
            return ReturnStatement(
                        GenerateCasesBinaryExpression(
                                @case,
                                BinaryExpression(
                                    SyntaxKind.EqualsExpression,
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName(Identifier("value")),
                                        IdentifierName(Identifier(GeneratorHelpers.TagFieldName))
                                    ),
                                    LiteralExpression(
                                        SyntaxKind.NumericLiteralExpression,
                                        Literal(@case.CaseNumber)
                                    )
                                ),
                                semanticModel
                            )
                        );
        }

        private ReturnStatementSyntax GenerateReturnStatementForSingleCase(IDiscriminatedUnionCase @case, SemanticModel semanticModel)
        {
            if (@case.CaseValues.IsEmpty)
            {
                return ReturnStatement(GeneratorHelpers.TrueExpression());
            }
            return ReturnStatement(GenerateCasesBinaryExpression(@case, GenerateCaseValueEqual(@case, 0, semanticModel), semanticModel, 1));
        }

        private ExpressionSyntax GenerateCasesBinaryExpression(
            IDiscriminatedUnionCase @case,
            ExpressionSyntax binaryExpressionSyntax,
            SemanticModel semanticModel,
            int caseValueIndex = 0)
        {
            if (@case.CaseValues.Length <= caseValueIndex)
            {
                return binaryExpressionSyntax;
            }

            var andExpression = BinaryExpression(
                SyntaxKind.LogicalAndExpression,
                binaryExpressionSyntax,
                 GenerateCaseValueEqual(@case, caseValueIndex, semanticModel)
            );
            return GenerateCasesBinaryExpression(@case, andExpression, semanticModel, caseValueIndex + 1);
        }

        private static ExpressionSyntax GenerateCaseValueEqual(IDiscriminatedUnionCase @case, int caseValueIndex, SemanticModel semanticModel)
        {
            if (GeneratorHelpers.IsStructuralEquatableType(@case.CaseValues[caseValueIndex], semanticModel))
            {
                return WrapEqualityWithNullGuard(@case.CaseValues[caseValueIndex], GenerateStructuralEquatableCaseValueEqual(@case.CaseValues[caseValueIndex]));
            }
            return GenerateDefaultCaseValueEqual(@case.CaseValues[caseValueIndex]);
        }

        private static ExpressionSyntax GenerateStructuralEquatableCaseValueEqual(CaseValue caseValue)
        {
            return InvocationExpression(
                        MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            ParenthesizedExpression(
                                CastExpression(
                                    GeneratorHelpers.StructuralEquatableName,
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        ThisExpression(),
                                        IdentifierName(caseValue.Name)
                                    )
                                )
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
                                            IdentifierName("value"),
                                            IdentifierName(caseValue.Name)
                                        )
                                    ),
                                    Token(SyntaxKind.CommaToken),
                                    Argument(GeneratorHelpers.StructuralEqualityComparerMemberAccess)
                                }
                            )
                        )
                    );
        }

        private static InvocationExpressionSyntax GenerateDefaultCaseValueEqual(CaseValue caseValue)
        {
            return InvocationExpression(
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
                    ).WithArgumentList(
                        ArgumentList(
                            SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    Argument(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            ThisExpression(),
                                            IdentifierName(caseValue.Name)
                                        )
                                    ),
                                    Token(SyntaxKind.CommaToken),
                                    Argument(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("value"),
                                            IdentifierName(caseValue.Name)
                                        )
                                    )
                                }
                            )
                        )
                    );
        }

        private static ExpressionSyntax WrapEqualityWithNullGuard(CaseValue caseValue, ExpressionSyntax expressionSyntax)
        {
            if (caseValue.SymbolInfo.IsValueType)
            {
                return expressionSyntax;
            }

            var leftSyntax = MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        ThisExpression(),
                                        IdentifierName(caseValue.Name)
                                    );
            var rightSyntax = MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("value"),
                                        IdentifierName(caseValue.Name)
                                    );
            return BinaryExpression(
                SyntaxKind.LogicalOrExpression,
                BinaryExpression(
                    SyntaxKind.LogicalAndExpression,
                    BinaryExpression(
                        SyntaxKind.EqualsExpression,
                        leftSyntax,
                        LiteralExpression(
                            SyntaxKind.NullLiteralExpression
                        )
                    ),
                    BinaryExpression(
                        SyntaxKind.EqualsExpression,
                        rightSyntax,
                        LiteralExpression(
                            SyntaxKind.NullLiteralExpression
                        )
                    )
                ),
                BinaryExpression(
                    SyntaxKind.LogicalAndExpression,
                    BinaryExpression(
                        SyntaxKind.NotEqualsExpression,
                        leftSyntax,
                        LiteralExpression(
                            SyntaxKind.NullLiteralExpression
                        )
                    ),
                    expressionSyntax
                )
            );
        }

    }
}
