using CSharpDiscriminatedUnion.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests.UnionTypes
{
    [GenerateDiscriminatedUnion(CaseFactoryPrefix = "Create")]
    public partial class OptionFactoryPrefix1
    {
        partial class Cases
        {
            partial class Case1 : OptionFactoryPrefix1 { readonly int value; }
        }
    }

    [GenerateDiscriminatedUnion(CaseFactoryPrefix = "New")]
    public partial class OptionFactoryPrefix2
    {
        partial class Cases
        {
            partial class Case1 : OptionFactoryPrefix2 { readonly int value; }
        }
    }

    [GenerateDiscriminatedUnion(CaseFactoryPrefix = "")]
    public partial class OptionFactoryPrefix3
    {
        partial class Cases
        {
            partial class Case1 : OptionFactoryPrefix3 { readonly int value; }
        }
    }

    [GenerateDiscriminatedUnion(CaseFactoryPrefix = null)]
    public partial class OptionFactoryPrefix4
    {
        partial class Cases
        {
            partial class Case1 : OptionFactoryPrefix4 { readonly int value; }
        }
    }
}
