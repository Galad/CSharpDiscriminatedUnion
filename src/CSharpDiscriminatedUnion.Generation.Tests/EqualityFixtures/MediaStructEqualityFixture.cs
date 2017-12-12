using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using System;
using System.Collections.Generic;

namespace CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures
{
    public class MediaStructEqualityFixture : UnionEqualityFixture<MediaStruct>
    {
        public override IEnumerable<Func<MediaStruct>> SameValues
        {
            get
            {
                yield return () => MediaStruct.NewBook("John Steinbeck", 464, "Grapes of the wraith");
                yield return () => MediaStruct.NewBook("Jules Verne", 500, "Twenty thousand leagues under the sea");
                yield return () => MediaStruct.NewMovie("Stanley Kubrick", TimeSpan.FromMinutes(85), "The killing");
                yield return () => MediaStruct.NewMovie("Frank Darabont", TimeSpan.FromMinutes(142), "The Shawshank Redemption");
                yield return () => MediaStruct.NewTvSeries("The handmaid's tale", 10);
                yield return () => MediaStruct.NewTvSeries("Quantum leap", 97);
            }
        }

        public override IEnumerable<(MediaStruct, MediaStruct)> DifferentValues
        {
            get
            {
                yield return (MediaStruct.NewBook("John Steinbeck", 464, "Grapes of the wraith"), MediaStruct.NewBook("Jules Verne", 500, "Twenty thousand leagues under the sea"));
                yield return (MediaStruct.NewBook("Jules Verne", 500, "Twenty thousand leagues under the sea"), MediaStruct.NewMovie("Stanley Kubrick", TimeSpan.FromMinutes(85), "The killing"));
                yield return (MediaStruct.NewMovie("Stanley Kubrick", TimeSpan.FromMinutes(85), "The killing"), MediaStruct.NewMovie("Frank Darabont", TimeSpan.FromMinutes(142), "The Shawshank Redemption"));
                yield return (MediaStruct.NewMovie("Frank Darabont", TimeSpan.FromMinutes(142), "The Shawshank Redemption"), MediaStruct.NewTvSeries("The handmaid's tale", 10));
                yield return (MediaStruct.NewTvSeries("The handmaid's tale", 10), MediaStruct.NewTvSeries("Quantum leap", 97));
                yield return (MediaStruct.NewTvSeries("Quantum leap", 97), MediaStruct.NewBook("John Steinbeck", 464, "Grapes of the wraith"));
            }
        }

        public override MediaStruct AnonymousValue => MediaStruct.NewBook("Philip K. Dick", 500, "Solar lottery");
    }
}
