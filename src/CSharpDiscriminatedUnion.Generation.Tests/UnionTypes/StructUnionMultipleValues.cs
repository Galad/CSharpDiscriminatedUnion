using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpDiscriminatedUnion.Attributes;

namespace CSharpDiscriminatedUnion.Generation.Tests.UnionTypes
{
    [GenerateDiscriminatedUnion]
    public partial struct StructBook
    {
        [StructCase("Book")]
        readonly string author;
        [StructCase("Book")]
        readonly int pageCount;
        [StructCase("Book")]
        readonly string title;
    }
}
