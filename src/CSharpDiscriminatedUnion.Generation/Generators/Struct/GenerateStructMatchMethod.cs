using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Linq;

namespace CSharpDiscriminatedUnion.Generation.Generators.Struct
{
    internal sealed class GenerateStructMatchMethod : IDiscriminatedUnionGenerator<StructDiscriminatedUnionCase>
    {
        public DiscriminatedUnionContext<StructDiscriminatedUnionCase> Build(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {
            if (context.Cases.IsEmpty)
            {
                return context;
            }
            return context.AddMember(GetMatchMethod(context));
        }

        private static MemberDeclarationSyntax GetMatchMethod(DiscriminatedUnionContext<StructDiscriminatedUnionCase> context)
        {
            var match = GeneratorHelpers.CreateMatchMethod(context.Cases.Cast<IDiscriminatedUnionCase>(), context.MatchGenericParameter)
                             .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PublicKeyword)
                                )
                             )
                             .WithBody(GetMatchBlock(context.Cases));
            return match;
        }

        private static BlockSyntax GetMatchBlock(ImmutableArray<StructDiscriminatedUnionCase> cases)
        {
            return Block(
                cases.Select(c => GeneratorHelpers.CreateGuardForNull(IdentifierName("match" + c.Name.Text)))
                .Concat(new[] { GenerateMatchStatement(cases) })
                .ToArray()
            );
        }

        private static StatementSyntax GenerateMatchStatement(ImmutableArray<StructDiscriminatedUnionCase> cases)
        {
            if(cases.Length == 1)
            {
                return GeneratorHelpers.GenerateMatchReturnStatement(cases[0]);
            }
            return GeneratorHelpers.GenerateStructMatchingSwitchStatement(cases.Cast<IDiscriminatedUnionCase>(), GeneratorHelpers.GenerateMatchReturnStatement);
        }
    }
}
