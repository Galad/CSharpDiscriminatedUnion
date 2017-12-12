using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    internal sealed class GenerateTagField<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        private static readonly FieldDeclarationSyntax TagField = CreateTagField();

        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {
            return context.AddMember(TagField);
        }

        private static FieldDeclarationSyntax CreateTagField()
        {
            return FieldDeclaration(VariableDeclaration(PredefinedType(Token(SyntaxKind.ByteKeyword)))
                                .WithVariables(
                                    SingletonSeparatedList(
                                        VariableDeclarator(Identifier("_tag"))
                                    )
                                ))
                                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword)));
        }
    }
}
