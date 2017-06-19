using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Validation;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    /// <summary>
    /// Build the factory methods for each case
    /// </summary>
    internal sealed class GenerateCaseFactoryMethods : IDiscriminatedUnionGenerator
    {
        private readonly string _prefix;
        private readonly bool _preventNull;

        public GenerateCaseFactoryMethods(string prefix, bool preventNull)
        {
            Requires.NotNull(prefix, nameof(prefix));
            _prefix = prefix;
            _preventNull = preventNull;
        }

        public DiscriminatedUnionContext Build(DiscriminatedUnionContext context)
        {
            return context.AddMembers(AddCasesMembers(context.Type, context.GeneratedPartialClass, context.Cases));
        }

        private IEnumerable<MemberDeclarationSyntax> AddCasesMembers(
            TypeSyntax type,
            ClassDeclarationSyntax partialClass,
            ImmutableArray<DiscriminatedUnionCase> cases)
        {
            return cases.Select(c =>
            {
                MemberDeclarationSyntax member;
                if (c.CaseValues.Length == 0)
                {
                    member = CreateSingleCaseSingleton(type, c);
                }
                else
                {
                    member = CreateCaseFactoryMethod(type, c);
                }
                return member;
            });
        }

        private static MemberDeclarationSyntax CreateSingleCaseSingleton(
            TypeSyntax type,
            DiscriminatedUnionCase singleCase)
        {
            return FieldDeclaration(
                        List<AttributeListSyntax>(),
                        TokenList(
                            Token(SyntaxKind.PublicKeyword),
                            Token(SyntaxKind.StaticKeyword),
                            Token(SyntaxKind.ReadOnlyKeyword)
                        ),
                        VariableDeclaration(
                            type,
                            SeparatedList(new[] {
                                VariableDeclarator(singleCase.Name)
                                .WithInitializer(
                                    EqualsValueClause(
                                        ObjectCreationExpression(
                                            QualifiedName(
                                                IdentifierName("Cases"),
                                                IdentifierName(singleCase.Name)
                                            )
                                        )
                                        .WithArgumentList(ArgumentList())
                                    )
                                )
                            })
                        )
                    );
        }

        private MemberDeclarationSyntax CreateCaseFactoryMethod(
            TypeSyntax type,
            DiscriminatedUnionCase singleCase)
        {
            return MethodDeclaration(
                            type,
                            _prefix + singleCase.Name)
                        .WithModifiers(
                            TokenList(
                                Token(SyntaxKind.PublicKeyword),
                                Token(SyntaxKind.StaticKeyword)
                            )
                        )
                        .WithParameterList(
                            ParameterList(
                                SeparatedList(
                                    singleCase.CaseValues.Select(p =>
                                        Parameter(p.Name).WithType(p.Type)
                                    )
                                )
                            )
                        )
                        .WithBody(
                            Block(
                                GenerateCreateCaseFactoryBlock(singleCase)
                            )
                        )
                        ;
        }

        private IEnumerable<StatementSyntax> GenerateCreateCaseFactoryBlock(DiscriminatedUnionCase singleCase)
        {
            if (_preventNull)
            {
                foreach (var caseValue in singleCase.CaseValues.Where(c => CanHaveNullGuard(c)))
                {
                    var c = caseValue;
                    yield return GeneratorHelpers.CreateGuardForNull(IdentifierName(c.Name));
                }
            }
            yield return ReturnStatement(
                    ObjectCreationExpression(
                        QualifiedName(
                            IdentifierName("Cases"),
                            IdentifierName(singleCase.Name)
                        )
                    )
                    .WithArgumentList(
                        ArgumentList(
                            SeparatedList(
                                singleCase.CaseValues.Select(p => Argument(IdentifierName(p.Name)))
                            )
                        )
                    )
                );
        }

        private static bool CanHaveNullGuard(CaseValue c)
        {
            return c.SymbolInfo.IsReferenceType ||
                   c.SymbolInfo.TypeKind == TypeKind.TypeParameter && 
                   !c.SymbolInfo.IsValueType;
        }
    }
}
