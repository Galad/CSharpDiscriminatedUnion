using CSharpDiscriminatedUnion.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generator.Tests.UnionTypes
{
#pragma warning disable CS0282 // There is no defined ordering between fields in multiple declarations of partial struct
    [GenerateDiscriminatedUnion]
    public partial struct IOStruct<T>
    {
        [StructCase("IO")]
        readonly T value;
    }

    [GenerateDiscriminatedUnion, System.Diagnostics.DebuggerDisplay("{DebugView}")]
    public partial struct EitherStruct<T>
    {
        [StructCase("Left")]
        readonly T valueLeft;
        [StructCase("Right")]
        readonly T valueRight;
    }

    [GenerateDiscriminatedUnion, System.Diagnostics.DebuggerDisplay("{DebugView}")]
    public partial struct EitherStruct2<TLeft, TRight>
    {
        [StructCase("Left")]
        readonly TLeft valueLeft;
        [StructCase("Right")]
        readonly TRight valueRight;
    }
#pragma warning restore CS0282 // There is no defined ordering between fields in multiple declarations of partial struct
}
