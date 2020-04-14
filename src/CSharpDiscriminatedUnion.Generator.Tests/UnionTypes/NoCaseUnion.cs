using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpDiscriminatedUnion.Attributes;

namespace CSharpDiscriminatedUnion.Generator.Tests.UnionTypes
{
    [GenerateDiscriminatedUnion]
    public partial class NoCaseUnion
    {
    }

    [GenerateDiscriminatedUnion]
    public partial class NoCaseUnionGeneric<T>
    {
    }

    [GenerateDiscriminatedUnion]
    public partial class NoCaseUnionGeneric<T, U>
    {
    }

    [GenerateDiscriminatedUnion]
    public partial class NoCaseUnionGeneric<T, U, V>
    {
    }

    [GenerateDiscriminatedUnion]
    public partial class NoCaseUnionGenericWithConstraints<T, U, V>
        where T : class
        where U : struct
        where V : List<string>, new()
    {
    }
}
