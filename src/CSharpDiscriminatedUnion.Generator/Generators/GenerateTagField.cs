using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpDiscriminatedUnion.Generator.Generators
{
    internal sealed class GenerateTagField<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        private static readonly FieldDeclarationSyntax TagField = CreateTagField();

        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {
            if(context.Cases.Length <= 1)
            {
                //we don't need a tag if there is zero or one case
                return context;
            }
            return context.AddMember(TagField);
        }

        private static FieldDeclarationSyntax CreateTagField()
        {
            return FieldDeclaration(VariableDeclaration(PredefinedType(Token(SyntaxKind.ByteKeyword)))
                                .WithVariables(
                                    SingletonSeparatedList(
                                        VariableDeclarator(Identifier(GeneratorHelpers.TagFieldName))
                                    )
                                ))
                                .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword), Token(SyntaxKind.ReadOnlyKeyword)));
        }
    }
}
