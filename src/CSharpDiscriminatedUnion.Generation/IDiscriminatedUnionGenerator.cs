using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation
{
    internal interface IDiscriminatedUnionGenerator
    {
        DiscriminatedUnionContext Build(DiscriminatedUnionContext context);
    }
}
