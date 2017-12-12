using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;

namespace CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures
{   
    public class StructBookEqualityFixture : UnionEqualityFixture<StructBook>
    {
        private static StructBook e(string author, int pageCount, string title) => StructBook.NewBook(author, pageCount, title);

        public override IEnumerable<Func<StructBook>> SameValues
        {
            get
            {
                yield return () => e("a", 1, "c");
                yield return () => e("b", 1, "b");
                yield return () => e("", 0, "");
            }
        }

        public override IEnumerable<(StructBook, StructBook)> DifferentValues
        {
            get
            {
                yield return (e("a", 1, "c"), e("a", 1, "d"));
                yield return (e("a", 0, "c"), e("a", 1, "c"));
                yield return (e("b", 1, "c"), e("a", 1, "c"));
                yield return (e("a", 1, "c"), e(null, 0, null));
            }
        }

        public override StructBook AnonymousValue => e("zzz", 1, "xxx");
    }
}
