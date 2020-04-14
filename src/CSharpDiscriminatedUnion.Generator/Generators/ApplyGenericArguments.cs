using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace CSharpDiscriminatedUnion.Generator.Generators
{
    /// <summary>
    /// Add the type arguments to the partial class when the user defined it as generic
    /// </summary>
    internal sealed class ApplyGenericArguments<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {            
            if (!context.IsGeneric)
            {
                return context;
            }
            var typedParameters = context.SymbolInfo
                                         .TypeParameters
                                         .Select(t => TypeParameter(t.Name));
            var newContext = context.WithTypeParameterList(
                TypeParameterList(
                    SeparatedList(typedParameters)
                )
            );
            return newContext;
        }
    }
}
