using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures
{
    public class Media_Collection_EqualityFixture<T> : UnionEqualityFixture<T>
    {
        private readonly Func<string[], T> createValue;

        public Media_Collection_EqualityFixture(Func<string[], T> createValue)
        {
            this.createValue = createValue;
        }

        public override IEnumerable<Func<T>> SameValues => new Func<T>[]
        {
            () => createValue(new string[]{ }),
            () => createValue(new string[]{ "a" }),
            () => createValue(new string[]{ "a", "b", "c" }),
        };

        public override IEnumerable<(T, T)> DifferentValues => new[]
        {
            (createValue(new string[]{ "a" }), createValue(new string[]{ })),
            (createValue(new string[]{ "a" }), createValue(new string[]{ "b" })),
            (createValue(new string[]{ "a", "b", "c" }), createValue(new string[]{ "b" })),
        };

        public override T AnonymousValue => createValue(new string[] { "a" });
    }

    public class Media_Collection_EqualityFixture : Media_Collection_EqualityFixture<Media_Collection>
    {
        public Media_Collection_EqualityFixture() : base(s => Media_Collection.NewTvSeries(new Collection<string>(s)))
        {
        }
    }

    public class Media_CollectionArray_EqualityFixture : Media_Collection_EqualityFixture<Media_CollectionArray>
    {
        public Media_CollectionArray_EqualityFixture() : base(Media_CollectionArray.NewTvSeries)
        {
        }
    }

    public class Media_CollectionList_EqualityFixture : Media_Collection_EqualityFixture<Media_CollectionList>
    {
        public Media_CollectionList_EqualityFixture() : base(s => Media_CollectionList.NewTvSeries(s.ToList()))
        {
        }
    }
    
    public class Media_CollectionImmutableArray_EqualityFixture : Media_Collection_EqualityFixture<Media_CollectionImmutableArray>
    {
        public Media_CollectionImmutableArray_EqualityFixture() : base(s => Media_CollectionImmutableArray.NewTvSeries(s.ToImmutableArray()))
        {
        }
    }

    public class MediaStruct_Collection_EqualityFixture : Media_Collection_EqualityFixture<MediaStruct_Collection>
    {
        public MediaStruct_Collection_EqualityFixture() : base(s => MediaStruct_Collection.NewTvSeries(new Collection<string>(s)))
        {
        }
    }

    public class MediaStruct_CollectionArray_EqualityFixture : Media_Collection_EqualityFixture<MediaStruct_CollectionArray>
    {
        public MediaStruct_CollectionArray_EqualityFixture() : base(MediaStruct_CollectionArray.NewTvSeries)
        {
        }
    }

    public class MediaStruct_CollectionList_EqualityFixture : Media_Collection_EqualityFixture<MediaStruct_CollectionList>
    {
        public MediaStruct_CollectionList_EqualityFixture() : base(s => MediaStruct_CollectionList.NewTvSeries(s.ToList()))
        {
        }
    }

    public class MediaStruct_CollectionImmutableArray_EqualityFixture : Media_Collection_EqualityFixture<MediaStruct_CollectionImmutableArray>
    {
        public MediaStruct_CollectionImmutableArray_EqualityFixture() : base(s => MediaStruct_CollectionImmutableArray.NewTvSeries(s.ToImmutableArray()))
        {
        }
    }
}
