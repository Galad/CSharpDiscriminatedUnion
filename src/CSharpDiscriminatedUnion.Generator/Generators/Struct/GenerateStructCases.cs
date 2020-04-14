using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpDiscriminatedUnion.Generator.Generators.Struct
{
    internal sealed class GenerateStructCases : IDiscriminatedUnionGenerator<StructDiscriminatedUnionCase>
    {
        public DiscriminatedUnionContext<StructDiscriminatedUnionCase> Build(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {
            return context.WithCases(CreateCases(context.Cases));
        }

        private ImmutableArray<StructDiscriminatedUnionCase> CreateCases(ImmutableArray<StructDiscriminatedUnionCase> cases)
        {
            return cases.Select(c =>
                    c.AddMember(
                       FieldDeclaration(
                           VariableDeclaration(
                               PredefinedType(Token(SyntaxKind.ByteKeyword))
                            )
                            .WithVariables(
                               SingletonSeparatedList(
                                   VariableDeclarator(
                                       c.Name
                                   )
                                   .WithInitializer(
                                       EqualsValueClause(
                                           LiteralExpression(
                                               SyntaxKind.NumericLiteralExpression,
                                               Literal(c.CaseNumber)
                                           )
                                       )
                                   )
                                )
                            )
                        )
                        .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.ConstKeyword)))
                    )
                )
                .ToImmutableArray();
        }
    }
}
