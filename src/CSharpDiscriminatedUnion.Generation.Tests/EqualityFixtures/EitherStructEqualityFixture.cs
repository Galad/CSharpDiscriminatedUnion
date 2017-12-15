using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures
{
    public class EitherStructIntEqualityFixture : UnionEqualityFixture<EitherStruct<int>>
    {
        public override IEnumerable<Func<EitherStruct<int>>> SameValues
        {
            get
            {
                yield return () => EitherStruct<int>.NewLeft(0);
                yield return () => EitherStruct<int>.NewLeft(1);
                yield return () => EitherStruct<int>.NewRight(0);
                yield return () => EitherStruct<int>.NewRight(1);
            }
        }

        public override IEnumerable<(EitherStruct<int>, EitherStruct<int>)> DifferentValues
        {
            get
            {
                yield return (EitherStruct<int>.NewLeft(0), EitherStruct<int>.NewRight(0));
                yield return (EitherStruct<int>.NewLeft(0), EitherStruct<int>.NewLeft(1));
                yield return (EitherStruct<int>.NewLeft(1), EitherStruct<int>.NewLeft(0));
                yield return (EitherStruct<int>.NewRight(0), EitherStruct<int>.NewRight(1));
                yield return (EitherStruct<int>.NewRight(1), EitherStruct<int>.NewRight(0));
            }
        }

        public override EitherStruct<int> AnonymousValue => EitherStruct<int>.NewRight(100);        
    }

    public class EitherStructStringEqualityFixture : UnionEqualityFixture<EitherStruct<string>>
    {
        public override IEnumerable<Func<EitherStruct<string>>> SameValues
        {
            get
            {
                yield return () => EitherStruct<string>.NewLeft("");
                yield return () => EitherStruct<string>.NewLeft("a");
                yield return () => EitherStruct<string>.NewRight("");
                yield return () => EitherStruct<string>.NewRight("a");
            }
        }

        public override IEnumerable<(EitherStruct<string>, EitherStruct<string>)> DifferentValues
        {
            get
            {
                yield return (EitherStruct<string>.NewLeft(""), EitherStruct<string>.NewRight(""));
                yield return (EitherStruct<string>.NewLeft(""), EitherStruct<string>.NewLeft("a"));
                yield return (EitherStruct<string>.NewLeft("a"), EitherStruct<string>.NewLeft(""));
                yield return (EitherStruct<string>.NewRight(""), EitherStruct<string>.NewRight("a"));
                yield return (EitherStruct<string>.NewRight("a"), EitherStruct<string>.NewRight(""));
            }
        }

        public override EitherStruct<string> AnonymousValue => EitherStruct<string>.NewRight("Hello");
    }
}
