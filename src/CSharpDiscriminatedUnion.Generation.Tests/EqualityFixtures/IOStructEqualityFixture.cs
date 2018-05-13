using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures
{
    public class IOStructIntEqualityFixture : UnionEqualityFixture<IOStruct<int>>
    {
        public override IEnumerable<Func<IOStruct<int>>> SameValues
        {
            get
            {
                yield return () => e(0);
                yield return () => e(1);
                yield return () => e(99);
            }
        }

        public override IEnumerable<(IOStruct<int>, IOStruct<int>)> DifferentValues
        {
            get
            {
                yield return (e(0), e(1));
                yield return (e(0), e(99));
                yield return (e(1), e(0));
            }
        }

        public override IOStruct<int> AnonymousValue => e(100);
        private static IOStruct<int> e(int value) => IOStruct<int>.NewIO(value);
    }

    public class IOStructStringEqualityFixture : UnionEqualityFixture<IOStruct<string>>
    {
        public override IEnumerable<Func<IOStruct<string>>> SameValues
        {
            get
            {
                yield return () => e("");
                yield return () => e("test");
                yield return () => e("Test");
            }
        }

        public override IEnumerable<(IOStruct<string>, IOStruct<string>)> DifferentValues
        {
            get
            {
                yield return (e(""), e("a"));
                yield return (e("a"), e(""));
                yield return (e("a"), e("b"));
            }
        }

        public override IOStruct<string> AnonymousValue => e("Hello");
        private static IOStruct<string> e(string value) => IOStruct<string>.NewIO(value);
    }
}
