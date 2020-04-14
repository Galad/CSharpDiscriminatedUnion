using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generator.Tests.EqualityFixtures
{     
    public class EitherStruct2IntIntEqualityFixture : UnionEqualityFixture<EitherStruct2<int, int>>
    {
        public override IEnumerable<Func<EitherStruct2<int, int>>> SameValues
        {
            get
            {
                yield return () => EitherStruct2<int, int>.NewLeft(0);
                yield return () => EitherStruct2<int, int>.NewLeft(1);
                yield return () => EitherStruct2<int, int>.NewRight(0);
                yield return () => EitherStruct2<int, int>.NewRight(1);
            }
        }

        public override IEnumerable<(EitherStruct2<int, int>, EitherStruct2<int, int>)> DifferentValues
        {
            get
            {
                yield return (EitherStruct2<int, int>.NewLeft(0), EitherStruct2<int, int>.NewRight(0));
                yield return (EitherStruct2<int, int>.NewLeft(0), EitherStruct2<int, int>.NewLeft(1));                
                yield return (EitherStruct2<int, int>.NewRight(0), EitherStruct2<int, int>.NewRight(1));                
            }
        }

        public override EitherStruct2<int, int> AnonymousValue => EitherStruct2<int, int>.NewRight(100);
    }

    public class EitherStruct2IntStringEqualityFixture : UnionEqualityFixture<EitherStruct2<int, string>>
    {
        public override IEnumerable<Func<EitherStruct2<int, string>>> SameValues
        {
            get
            {
                yield return () => EitherStruct2<int, string>.NewLeft(0);
                yield return () => EitherStruct2<int, string>.NewLeft(1);
                yield return () => EitherStruct2<int, string>.NewRight("");
                yield return () => EitherStruct2<int, string>.NewRight("a");
            }
        }

        public override IEnumerable<(EitherStruct2<int, string>, EitherStruct2<int, string>)> DifferentValues
        {
            get
            {
                yield return (EitherStruct2<int, string>.NewLeft(0), EitherStruct2<int, string>.NewRight(""));
                yield return (EitherStruct2<int, string>.NewLeft(0), EitherStruct2<int, string>.NewLeft(1));                
                yield return (EitherStruct2<int, string>.NewRight(""), EitherStruct2<int, string>.NewRight("a"));                
            }
        }

        public override EitherStruct2<int, string> AnonymousValue => EitherStruct2<int, string>.NewRight("Hello");
    }

    public class EitherStruct2StringIntEqualityFixture : UnionEqualityFixture<EitherStruct2<string, int>>
    {
        public override IEnumerable<Func<EitherStruct2<string, int>>> SameValues
        {
            get
            {
                yield return () => EitherStruct2<string, int>.NewLeft("z");
                yield return () => EitherStruct2<string, int>.NewLeft("a");
                yield return () => EitherStruct2<string, int>.NewRight(0);
                yield return () => EitherStruct2<string, int>.NewRight(1);
            }
        }

        public override IEnumerable<(EitherStruct2<string, int>, EitherStruct2<string, int>)> DifferentValues
        {
            get
            {
                yield return (EitherStruct2<string, int>.NewLeft("z"), EitherStruct2<string, int>.NewRight(0));
                yield return (EitherStruct2<string, int>.NewLeft("z"), EitherStruct2<string, int>.NewLeft("a"));                
                yield return (EitherStruct2<string, int>.NewRight(0), EitherStruct2<string, int>.NewRight(1));                
            }
        }

        public override EitherStruct2<string, int> AnonymousValue => EitherStruct2<string, int>.NewRight(100);
    }

    public class EitherStruct2StringStringEqualityFixture : UnionEqualityFixture<EitherStruct2<string, string>>
    {
        public override IEnumerable<Func<EitherStruct2<string, string>>> SameValues
        {
            get
            {
                yield return () => EitherStruct2<string, string>.NewLeft("");
                yield return () => EitherStruct2<string, string>.NewLeft("a");
                yield return () => EitherStruct2<string, string>.NewRight("");
                yield return () => EitherStruct2<string, string>.NewRight("a");
            }
        }

        public override IEnumerable<(EitherStruct2<string, string>, EitherStruct2<string, string>)> DifferentValues
        {
            get
            {
                yield return (EitherStruct2<string, string>.NewLeft(""), EitherStruct2<string, string>.NewRight(""));
                yield return (EitherStruct2<string, string>.NewLeft(""), EitherStruct2<string, string>.NewLeft("a"));                
                yield return (EitherStruct2<string, string>.NewRight(""), EitherStruct2<string, string>.NewRight("a"));                
            }
        }

        public override EitherStruct2<string, string> AnonymousValue => EitherStruct2<string, string>.NewRight("Hello");
    }
}
