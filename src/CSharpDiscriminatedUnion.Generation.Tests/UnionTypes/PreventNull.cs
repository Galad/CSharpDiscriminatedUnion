using CSharpDiscriminatedUnion.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests.UnionTypes
{
    [GenerateDiscriminatedUnion(PreventNullValues = true)]
    public partial class PreventNull1
    {
        partial class Cases
        {
            partial class Case1 : PreventNull1
            {
                readonly string value;
            }

            partial class Case2 : PreventNull1
            {
                readonly int value;
            }
        }
    }

    [GenerateDiscriminatedUnion(PreventNullValues = true)]
    public partial class PreventNull2
    {
        partial class Cases
        {
            partial class Case1 : PreventNull2
            {
                readonly string value;
                readonly object value2;
                readonly int value4;
            }
        }
    }


    [GenerateDiscriminatedUnion(PreventNullValues = true)]
    public partial class PreventNull3<T>
    {
        partial class Cases
        {
            partial class Case1 : PreventNull3<T>
            {
                readonly T value;
            }
        }
    }


    [GenerateDiscriminatedUnion(PreventNullValues = true)]
    public partial class PreventNull4<T> where T : struct
    {
        partial class Cases
        {
            partial class Case1 : PreventNull4<T>
            {
                readonly T value;
            }
        }
    }


    [GenerateDiscriminatedUnion(PreventNullValues = true)]
    public partial class PreventNull5<T> where T : class
    {
        partial class Cases
        {
            partial class Case1 : PreventNull5<T>
            {
                readonly T value;
            }
        }
    }
}
