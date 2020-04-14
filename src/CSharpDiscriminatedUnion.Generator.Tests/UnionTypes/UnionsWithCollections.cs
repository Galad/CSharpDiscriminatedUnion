using CSharpDiscriminatedUnion.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generator.Tests.UnionTypes
{
    [GenerateDiscriminatedUnion]
    public partial class Media_Collection
    {
        partial class Cases
        {
            partial class TvSeries : Media_Collection
            {
                readonly Collection<string> episodes;
            }
        }
    }

    [GenerateDiscriminatedUnion]
    public partial class Media_CollectionArray
    {
        partial class Cases
        {
            partial class TvSeries : Media_CollectionArray
            {
                readonly string[] episodes;
            }
        }
    }

    [GenerateDiscriminatedUnion]
    public partial class Media_CollectionList
    {
        partial class Cases
        {
            partial class TvSeries : Media_CollectionList
            {
                readonly List<string> episodes;
            }
        }
    }

    [GenerateDiscriminatedUnion]
    public partial class Media_CollectionImmutableArray
    {
        partial class Cases
        {
            partial class TvSeries : Media_CollectionImmutableArray
            {
                readonly ImmutableArray<string> episodes;
            }
        }
    }

    [GenerateDiscriminatedUnion]
    public partial struct MediaStruct_Collection
    {
        [StructCase("TvSeries")]
        readonly Collection<string> episodes;
    }

    [GenerateDiscriminatedUnion]
    public partial struct MediaStruct_CollectionArray
    {
        [StructCase("TvSeries")]
        readonly string[] episodes;
    }

    [GenerateDiscriminatedUnion]
    public partial struct MediaStruct_CollectionList
    {
        [StructCase("TvSeries")]
        readonly List<string> episodes;
    }

    [GenerateDiscriminatedUnion]
    public partial struct MediaStruct_CollectionImmutableArray
    {
        [StructCase("TvSeries")]
        readonly ImmutableArray<string> episodes;
    }
}
