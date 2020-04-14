using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace CSharpDiscriminatedUnion.Generator.Generators.Class
{
    /// <summary>
    /// Generates the abstract Match method
    /// </summary>
    internal class GenerateAbstractMatchMethod : IDiscriminatedUnionGenerator<DiscriminatedUnionCase>
    {
        public DiscriminatedUnionContext<DiscriminatedUnionCase> Build(DiscriminatedUnionContext<DiscriminatedUnionCase> context)
        {
            if (context.Cases.IsEmpty)
            {
                return context;
            }
            return context.AddMember(GetAbstractMatchMethod(context));
        }

        private static MemberDeclarationSyntax GetAbstractMatchMethod(DiscriminatedUnionContext<DiscriminatedUnionCase> context)
        {                 
            var match = GeneratorHelpers.CreateMatchMethod(context.Cases.Cast<IDiscriminatedUnionCase>(), context.MatchGenericParameter)
                             .WithModifiers(
                                TokenList(
                                    Token(SyntaxKind.PublicKeyword),
                                    Token(SyntaxKind.AbstractKeyword)
                                )
                             )
                             .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
            return match;
        }
    }
}
