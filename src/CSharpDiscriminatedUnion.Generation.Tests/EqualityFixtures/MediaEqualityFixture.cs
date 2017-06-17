using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures
{
    public class MediaEqualityFixture : UnionEqualityFixture<Media>
    {
        public override IEnumerable<Func<Media>> SameValues
        {
            get
            {
                yield return () => Media.NewBook("John Steinbeck", 464, "Grapes of the wraith");
                yield return () => Media.NewBook("Jules Verne", 500, "Twenty thousand leagues under the sea");
                yield return () => Media.NewMovie("Stanley Kubrick", TimeSpan.FromMinutes(85), "The killing");
                yield return () => Media.NewMovie("Frank Darabont", TimeSpan.FromMinutes(142), "The Shawshank Redemption");
                yield return () => Media.NewTvSeries("The handmaid's tale", 10);
                yield return () => Media.NewTvSeries("Quantum leap", 97);
            }
        }

        public override IEnumerable<(Media, Media)> DifferentValues
        {
            get
            {
                yield return (Media.NewBook("John Steinbeck", 464, "Grapes of the wraith"), Media.NewBook("Jules Verne", 500, "Twenty thousand leagues under the sea"));
                yield return (Media.NewBook("Jules Verne", 500, "Twenty thousand leagues under the sea"), Media.NewMovie("Stanley Kubrick", TimeSpan.FromMinutes(85), "The killing"));
                yield return (Media.NewMovie("Stanley Kubrick", TimeSpan.FromMinutes(85), "The killing"), Media.NewMovie("Frank Darabont", TimeSpan.FromMinutes(142), "The Shawshank Redemption"));
                yield return (Media.NewMovie("Frank Darabont", TimeSpan.FromMinutes(142), "The Shawshank Redemption"), Media.NewTvSeries("The handmaid's tale", 10));
                yield return (Media.NewTvSeries("The handmaid's tale", 10), Media.NewTvSeries("Quantum leap", 97));
                yield return (Media.NewTvSeries("Quantum leap", 97), Media.NewBook("John Steinbeck", 464, "Grapes of the wraith"));
            }
        }

        public override Media AnonymousValue => Media.NewBook("Philip K. Dick", 500, "Solar lottery");
    }
}
