using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    /// <summary>
    /// Generate the implementation of the <see cref="IEquatable.Equals"/> method
    /// </summary>
    internal class GenerateAbstractEquatableImplementation<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {
            return context.AddMember(AddEqualityImplementations(context.Type));
        }

        private static MemberDeclarationSyntax AddEqualityImplementations(
            TypeSyntax applyToClassType)
        {
            return GeneratorHelpers.GenerateEquatableImplementation(applyToClassType, "value")
                                   .WithModifiers(TokenList(Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.AbstractKeyword)))
                                   .WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
        }

    }
}
