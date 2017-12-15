using CSharpDiscriminatedUnion.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests.UnionTypes
{
    [GenerateDiscriminatedUnion]
    public partial struct IOStruct<T>
    {
        [StructCase("IO")]
        readonly T value;
    }

    [GenerateDiscriminatedUnion]
    public partial struct EitherStruct<T>
    {
        [StructCase("Left")]
        readonly T valueLeft;
        [StructCase("Right")]
        readonly T valueRight;
    }

    [GenerateDiscriminatedUnion]
    public partial struct EitherStruct2<TLeft, TRight>
    {
        [StructCase("Left")]
        readonly TLeft valueLeft;
        [StructCase("Right")]
        readonly TRight valueRight;
    }
}
