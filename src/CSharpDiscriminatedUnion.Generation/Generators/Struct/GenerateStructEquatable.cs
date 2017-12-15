using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpDiscriminatedUnion.Generation.Generators.Struct
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
            yield return SwitchStatement(
                MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    ThisExpression(),
                    IdentifierName(Identifier("_tag"))
                )                
            )
            .WithSections(List(GenerateSwitchSections(context)));
        }

        private IEnumerable<SwitchSectionSyntax> GenerateSwitchSections(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {
            return context.Cases.Select(@case =>
                SwitchSection(
                    SingletonList<SwitchLabelSyntax>(CaseSwitchLabel(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(@case.CaseNumber)))),
                    SingletonList<StatementSyntax>(
                        ReturnStatement(                           
                            GenerateCasesBinaryExpression(
                                    @case,
                                    BinaryExpression(
                                    SyntaxKind.EqualsExpression,
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName(Identifier("value")),
                                        IdentifierName(Identifier("_tag"))
                                    ),
                                    LiteralExpression(
                                        SyntaxKind.NumericLiteralExpression,
                                        Literal(@case.CaseNumber)
                                    )
                                )
                            )                           
                        )
                    )
                )                
            )
            .Concat(new[]{
                SwitchSection().WithLabels(
                    SingletonList<SwitchLabelSyntax>(
                        DefaultSwitchLabel()
                    )
                )
                .WithStatements(
                    SingletonList<StatementSyntax>(
                        ThrowStatement(
                            ObjectCreationExpression(
                                QualifiedName(
                                    IdentifierName("System"),
                                    IdentifierName("ArgumentOutOfRangeException")
                                )
                            )
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(
                                            LiteralExpression(
                                                SyntaxKind.StringLiteralExpression,
                                                Literal("The given value does not represent a known case")
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            });
        }

        private ExpressionSyntax GenerateCasesBinaryExpression(
            StructDiscriminatedUnionCase @case,
            BinaryExpressionSyntax binaryExpressionSyntax,
            int caseValueIndex = 0)
        {
            if(@case.CaseValues.Length <= caseValueIndex)
            {
                return binaryExpressionSyntax;
            }
           
            var andExpression = BinaryExpression(
                SyntaxKind.LogicalAndExpression,
                binaryExpressionSyntax,
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
                                        SingletonSeparatedList<TypeSyntax>(@case.CaseValues[caseValueIndex].Type)
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
                            new SyntaxNodeOrToken[]{
                                Argument(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        ThisExpression(),
                                        IdentifierName(@case.CaseValues[caseValueIndex].Name)
                                    )
                                ),
                                Token(SyntaxKind.CommaToken),
                                Argument(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("value"),
                                        IdentifierName(@case.CaseValues[caseValueIndex].Name)
                                    )
                                )
                            }
                        )
                    )
                )
            );
            return GenerateCasesBinaryExpression(@case, andExpression, caseValueIndex + 1);
        }
    }
}
