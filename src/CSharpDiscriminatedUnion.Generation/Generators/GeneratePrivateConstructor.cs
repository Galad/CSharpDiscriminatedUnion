using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    /// <summary>
    /// Generates a private constructor so that the class cannot be inherited by consumers
    /// </summary>
    internal sealed class GeneratePrivateConstructor<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {
            var constructor = ConstructorDeclaration(context.UserDefinedClass.Identifier)
                        .AddModifiers(Token(SyntaxKind.PrivateKeyword))
                        .WithBody(Block());
            return context.AddMember(constructor);
        }
    }
}
