using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpDiscriminatedUnion.Generation.Generators.Class
{
    /// <summary>
    /// Generate the Match implementation for the case classes
    /// </summary>
    internal class GenerateMatchImplementation : IDiscriminatedUnionGenerator<DiscriminatedUnionCase>
    {
        public DiscriminatedUnionContext<DiscriminatedUnionCase> Build(DiscriminatedUnionContext<DiscriminatedUnionCase> context)
        {
            if (context.Cases.IsEmpty)
            {
                return context;
            }
            var newCases = context.Cases.Select(c => c.AddMember(GetMatchImplementation(context, c)))
                                        .ToImmutableArray();
            return context.WithCases(newCases);
        }

        private static MemberDeclarationSyntax GetMatchImplementation(
            DiscriminatedUnionContext<DiscriminatedUnionCase> context,
            DiscriminatedUnionCase currentCase)
        {
            var match = GeneratorHelpers.CreateMatchMethod(context.Cases, context.MatchGenericParameter)
                             .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.OverrideKeyword)
                                )
                             )
                             .WithBody(GetMatchBlock(context.Cases, currentCase));
            return match;
        }

        private static BlockSyntax GetMatchBlock(
            ImmutableArray<DiscriminatedUnionCase> cases,
            DiscriminatedUnionCase currentCase)
        {
            StatementSyntax getReturnStatement()
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
            };
            return Block(
                cases.Select(c => GeneratorHelpers.CreateGuardForNull(IdentifierName("match" + c.Name.Text)))
                .Concat(new[] { getReturnStatement() })
                );
        }
    }
}
