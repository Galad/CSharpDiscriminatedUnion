using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    internal static partial class GeneratorHelpers
    {
        public const string TagFieldName = "_tag";
        public static NameSyntax StructuralEquatableName { get; } = QualifiedName(
                                        QualifiedName(
                                            IdentifierName("System"),
                                            IdentifierName("Collections")
                                        ),
                                        IdentifierName("IStructuralEquatable")
                                    );
        public static ExpressionSyntax StructuralEqualityComparerMemberAccess { get; } = MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            MemberAccessExpression(
                                                SyntaxKind.SimpleMemberAccessExpression,
                                                MemberAccessExpression(
                                                    SyntaxKind.SimpleMemberAccessExpression,
                                                    IdentifierName("System"),
                                                    IdentifierName("Collections")
                                                ),
                                                IdentifierName("StructuralComparisons")
                                            ),
                                            IdentifierName("StructuralEqualityComparer")
                                        );

        public static MethodDeclarationSyntax CreateMatchMethod(IEnumerable<IDiscriminatedUnionCase> cases, SyntaxToken generateParameterName)
        {            
            var match =
                MethodDeclaration(IdentifierName(generateParameterName), "Match")
                             .AddTypeParameterListParameters(TypeParameter(generateParameterName))
                             .AddParameterListParameters(cases.Select(c => GetCaseMatchFunction(c, generateParameterName)).ToArray());
            return match;
        }

        public static MethodDeclarationSyntax CreateMatchDefaultCaseMethod(IEnumerable<IDiscriminatedUnionCase> cases, SyntaxToken generateParameterName)
        {
            var caseParameters = cases.Select(c => defaultNullParameter(GetCaseMatchFunction(c, generateParameterName)));
            var noneParameter = Parameter(Identifier("none"))
                                    .WithType(
                                        GenericName(
                                            Identifier("System.Func"),
                                            TypeArgumentList(
                                                SeparatedList(new TypeSyntax[] { IdentifierName(generateParameterName) })
                                            )
                                        )
                                    );
            var allParameters = new[] { noneParameter }.Concat(caseParameters).ToArray();
            var match =
                MethodDeclaration(IdentifierName(generateParameterName), "Match")
                             .AddTypeParameterListParameters(TypeParameter(generateParameterName))
                             .AddParameterListParameters(allParameters);
                        
            return match;

            ParameterSyntax defaultNullParameter(ParameterSyntax parameter)
            {
                return parameter.WithDefault(SyntaxFactory.EqualsValueClause(NullExpression()));
            }
        }

        private static ParameterSyntax GetCaseMatchFunction(IDiscriminatedUnionCase @case, SyntaxToken generateParameterName)
        {
            return Parameter(
                    Identifier("match" + @case.Name.Text)
                    )
                    .WithType(
                        GenericName(
                            Identifier("System.Func"),
                            TypeArgumentList(
                                SeparatedList(
                                    @case.CaseValues.Select(vc => vc.Type).Concat(new[] { IdentifierName(generateParameterName) })
                                )
                            )
                        )
                    );
        }

        public static StatementSyntax CreateGuardForNull(IdentifierNameSyntax identifier)
        {
            return IfStatement(
                BinaryExpression(
                        SyntaxKind.EqualsExpression,
                        identifier,
                        LiteralExpression(
                            SyntaxKind.NullLiteralExpression
                        )
                    ),
                Block(
                    SingletonList<StatementSyntax>(
                        ThrowStatement(
                            ObjectCreationExpression(
                                QualifiedName(
                                    IdentifierName("System"),
                                    IdentifierName("ArgumentNullException")
                                )
                            )
                            .WithArgumentList(
                                ArgumentList(
                                    SingletonSeparatedList(
                                        Argument(
                                            InvocationExpression(
                                                IdentifierName("nameof")
                                            )
                                            .WithArgumentList(
                                                ArgumentList(
                                                    SingletonSeparatedList(
                                                        Argument(identifier)
                                                    )
                                                )
                                            )
                                        )
                                    )
                                )
                            )
                        )
                    )
                )
            );
        }
                
        public static MethodDeclarationSyntax GenerateEquatableImplementation(TypeSyntax applyToClassType, string parameterName)
        {
            return MethodDeclaration(
                                PredefinedType(Token(SyntaxKind.BoolKeyword)),
                                Identifier("Equals")
                            )
                            .WithParameterList(
                                ParameterList(
                                    SingletonSeparatedList(
                                        Parameter(Identifier(parameterName))
                                            .WithType(applyToClassType)
                                    )
                                )
                            );
        }

        public static InvocationExpressionSyntax InvokeReferenceEquals(ExpressionSyntax left, ExpressionSyntax right)
        {
            return InvocationExpression(IdentifierName("ReferenceEquals"))
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList<ArgumentSyntax>(
                                new SyntaxNodeOrToken[]
                                {
                                    Argument(left),
                                    Token(SyntaxKind.CommaToken),
                                    Argument(right)
                                }
                            )
                        )
                    );
        }

        public static StatementSyntax ReturnFalse()
        {
            return Block(
                SingletonList<StatementSyntax>(
                    ReturnStatement(FalseExpression())
                )
            );
        }

        public static StatementSyntax ReturnTrue()
        {
            return Block(
                SingletonList<StatementSyntax>(
                    ReturnStatement(TrueExpression())
                )
            );
        }

        public static LiteralExpressionSyntax FalseExpression()
        {
            return LiteralExpression(SyntaxKind.FalseLiteralExpression, Token(SyntaxKind.FalseKeyword));
        }

        public static LiteralExpressionSyntax TrueExpression()
        {
            return LiteralExpression(SyntaxKind.TrueLiteralExpression, Token(SyntaxKind.TrueKeyword));
        }

        public static LiteralExpressionSyntax NullExpression()
        {
            return LiteralExpression(SyntaxKind.NullLiteralExpression, Token(SyntaxKind.NullKeyword));
        }

        private static readonly ImmutableArray<string> _matchGenericParametersCandidates = ImmutableArray.Create("T", "TResult", "TMatch", "TMatchResult");
        private const string GeneratedMatchGenericParameter = "TResult";

        public static SyntaxToken GenerateMatchResultGenericParameterName(TypeDeclarationSyntax @class, bool isGeneric)
        {
            if (!isGeneric)
            {
                return Identifier("T");
            }
            var genericParameterNames = @class.TypeParameterList.Parameters.Select(p => p.Identifier.ValueText).ToImmutableHashSet();

            foreach (var candidate in _matchGenericParametersCandidates)
            {
                if (!genericParameterNames.Contains(candidate))
                {
                    return Identifier(candidate);
                }
            }
            int i;
            for (i = 1; !genericParameterNames.Contains(GeneratedMatchGenericParameter + i.ToString()); i++)
            {
            }
            return Identifier(GeneratedMatchGenericParameter + i.ToString());
        }

        public static readonly MethodDeclarationSyntax EqualOverrideMethodDeclarationSyntax = MethodDeclaration(
                PredefinedType(Token(SyntaxKind.BoolKeyword)),
                "Equals"
            )
            .WithParameterList(
                ParameterList(
                    SingletonSeparatedList(
                        Parameter(Identifier("obj"))
                        .WithType(
                            PredefinedType(
                                Token(SyntaxKind.ObjectKeyword)
                            )
                        )
                    )
                )
            )
            .WithModifiers(
                TokenList(
                    Token(SyntaxKind.PublicKeyword),
                    Token(SyntaxKind.OverrideKeyword)
                )
            );

        public static SwitchStatementSyntax GenerateStructMatchingSwitchStatement(
            IEnumerable<IDiscriminatedUnionCase> cases,
            Func<IDiscriminatedUnionCase, ReturnStatementSyntax> generateReturnStatement)
        {
            IEnumerable<SwitchSectionSyntax> generateStructMatchingImplementation()
            {
                return cases.Select(@case =>
                    SwitchSection(
                        SingletonList<SwitchLabelSyntax>(CaseSwitchLabel(LiteralExpression(SyntaxKind.NumericLiteralExpression, Literal(@case.CaseNumber)))),
                        SingletonList<StatementSyntax>(
                            generateReturnStatement(@case)
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
            
            return SwitchStatement(
                    MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        ThisExpression(),
                        IdentifierName(Identifier(GeneratorHelpers.TagFieldName))
                    )
                )
                .WithSections(List(generateStructMatchingImplementation()));
        }

        public static ReturnStatementSyntax GenerateMatchReturnStatement(IDiscriminatedUnionCase currentCase)
        {
            return ReturnStatement(
                 InvocationExpression(
                     IdentifierName("match" + currentCase.Name)
                 )
                 .WithArgumentList(
                     ArgumentList(
                         SeparatedList(
                            currentCase.CaseValues.Select(v =>
                                 Argument(
                                     MemberAccessExpression(
                                         SyntaxKind.SimpleMemberAccessExpression,
                                         ThisExpression(),
                                         IdentifierName(v.Name)
                                     )
                                 )
                             )
                         )
                     )
                 )
             );
        }

        public static bool IsStructuralEquatableType(CaseValue caseValue, SemanticModel semanticModel)
        {
            var structuralEquatableMembers = semanticModel.Compilation.GetTypeByMetadataName("System.Collections.IStructuralEquatable").GetMembers();
            var type = caseValue.SymbolInfo;            
            return structuralEquatableMembers.Any(m => type.FindImplementationForInterfaceMember(m) != null);
        }

        private static readonly AttributeListSyntax _debuggerDisplayAttribute = AttributeList(
                                SingletonSeparatedList(
                                    Attribute(
                                        QualifiedName(
                                            QualifiedName(
                                                IdentifierName("System"),
                                                IdentifierName("Diagnostics")
                                            ),
                                            IdentifierName("DebuggerDisplay")
                                        )
                                    )
                                    .WithArgumentList(
                                        AttributeArgumentList(
                                            SeparatedList<AttributeArgumentSyntax>(
                                                new SyntaxNodeOrToken[]{
                                                    AttributeArgument(
                                                        LiteralExpression(
                                                            SyntaxKind.StringLiteralExpression,
                                                            Literal("{DebugView}")
                                                        )
                                                    )
                                                }
                                            )
                                        )
                                    )
                                )
                            );

        public static AttributeListSyntax CreateDebuggerDisplayAttributeList() => _debuggerDisplayAttribute;
    }
}
