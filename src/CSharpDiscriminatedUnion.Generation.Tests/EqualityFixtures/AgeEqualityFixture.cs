using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures
{
    public class AgeEqualityFixture : UnionEqualityFixture<Age>
    {
        public override IEnumerable<Func<Age>> SameValues
        {
            get
            {
                yield return () => e(0);
                yield return () => e(10);
                yield return () => e(int.MinValue);
            }
        }

        public override IEnumerable<(Age, Age)> DifferentValues
        {
            get
            {
                yield return (e(0), e(1));
                yield return (e(1), e(10));
                yield return (e(int.MinValue), e(int.MaxValue));
            }
        }

        public override Age AnonymousValue => e(30);
        
        private static Age e(int age) => Age.NewAge(age);
    }
}
