using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    /// <summary>
    /// Generates the abstract Match method
    /// </summary>
    internal class GenerateAbstractMatchMethod : IDiscriminatedUnionGenerator<ClassDiscriminatedUnionCase>
    { 
        public DiscriminatedUnionContext<ClassDiscriminatedUnionCase> Build(DiscriminatedUnionContext<ClassDiscriminatedUnionCase> context)
        {
            if (context.Cases.IsEmpty)
            {
                return context;
            }
            return context.AddMember(GetAbstractMatchMethod(context));
        }

        private static MemberDeclarationSyntax GetAbstractMatchMethod(DiscriminatedUnionContext<ClassDiscriminatedUnionCase> context)
        {
            var match =GeneratorHelpers.CreateMatchMethod(context.Cases, context.MatchGenericParameter)
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
