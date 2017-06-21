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
using CSharpDiscriminatedUnion.Generation;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    /// <summary>
    /// Build the factory methods for each case
    /// </summary>
    internal class GenerateCaseFactoryMethods<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        private readonly string _prefix;
        private readonly bool _preventNull;
        private readonly Func<DiscriminatedUnionContext<T>, T, ExpressionSyntax> _singletonInitializer;
        private readonly Func<DiscriminatedUnionContext<T>, T, ExpressionSyntax> _generateFactoryMethodReturnStatement;        

        public GenerateCaseFactoryMethods(
            string prefix,
            bool preventNull,
            Func<DiscriminatedUnionContext<T>, T, ExpressionSyntax> singletonInitializer,
            Func<DiscriminatedUnionContext<T>, T, ExpressionSyntax> generateFactoryMethodReturnStatement)
        {
            Requires.NotNull(prefix, nameof(prefix));
            _prefix = prefix;
            _preventNull = preventNull;
            _singletonInitializer = singletonInitializer;
            _generateFactoryMethodReturnStatement = generateFactoryMethodReturnStatement;
        }
        
        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {
            return context.AddMembers(AddCasesMembers(context));
        }

        private IEnumerable<MemberDeclarationSyntax> AddCasesMembers(DiscriminatedUnionContext<T> context)
        {
            return context.Cases.Select(c =>
            {
                MemberDeclarationSyntax member;
                if (c.CaseValues.Length == 0)
                {
                    member = CreateSingleCaseSingleton(context, c);
                }
                else
                {
                    member = CreateCaseFactoryMethod(context, c);
                }
                return member;
            });
        }

        private MemberDeclarationSyntax CreateSingleCaseSingleton(
            DiscriminatedUnionContext<T> context,
            T singleCase)
        {
            return FieldDeclaration(
                        List<AttributeListSyntax>(),
                        TokenList(
                            Token(SyntaxKind.PublicKeyword),
                            Token(SyntaxKind.StaticKeyword),
                            Token(SyntaxKind.ReadOnlyKeyword)
                        ),
                        VariableDeclaration(
                            context.Type,
                            SeparatedList(new[] {
                                VariableDeclarator(singleCase.Name)
                                .WithInitializer(
                                    EqualsValueClause(
                                        _singletonInitializer(context, singleCase)
                                    )
                                )
                            })
                        )
                    );
        }

        private MemberDeclarationSyntax CreateCaseFactoryMethod(
            DiscriminatedUnionContext<T> context,
            T singleCase)
        {
            return MethodDeclaration(
                            context.Type,
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
                                GenerateCreateCaseFactoryBlock(context, singleCase)
                            )
                        )
                        ;
        }

        private IEnumerable<StatementSyntax> GenerateCreateCaseFactoryBlock(
            DiscriminatedUnionContext<T> context, 
            T singleCase)
        {
            if (_preventNull)
            {
                foreach (var caseValue in singleCase.CaseValues.Where(c => CanHaveNullGuard(c)))
                {
                    var c = caseValue;
                    yield return GeneratorHelpers.CreateGuardForNull(IdentifierName(c.Name));
                }
            }
            yield return ReturnStatement(_generateFactoryMethodReturnStatement(context, singleCase));
        }

        private static bool CanHaveNullGuard(CaseValue c)
        {
            return c.SymbolInfo.IsReferenceType ||
                   c.SymbolInfo.TypeKind == TypeKind.TypeParameter &&
                   !c.SymbolInfo.IsValueType;
        }
    }
}
