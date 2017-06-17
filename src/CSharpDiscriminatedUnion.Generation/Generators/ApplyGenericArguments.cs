using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpDiscriminatedUnion.Generation.Generators
{
    /// <summary>
    /// Add the type arguments to the partial class when the user defined it as generic
    /// </summary>
    internal sealed class ApplyGenericArguments : IDiscriminatedUnionGenerator
    {
        public DiscriminatedUnionContext Build(DiscriminatedUnionContext context)
        {
            if (!context.IsGeneric)
            {
                return context;
            }
            var typedParameters = context.SymbolInfo
                                         .TypeParameters
                                         .Select(t => TypeParameter(t.Name));
            var newPartialClass = context.GeneratedPartialClass.WithTypeParameterList(
                TypeParameterList(
                    SeparatedList(typedParameters)
                )
            );
            return context.WithGeneratedPartialClass(newPartialClass);
        }
    }
}
