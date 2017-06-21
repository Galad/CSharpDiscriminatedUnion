using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using System;
using System.Collections.Generic;

namespace CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures
{
    public class BooleanUnionStructEqualityFixture : UnionEqualityFixture<BooleanUnion>
    {
        public override IEnumerable<Func<BooleanUnion>> SameValues
        {
            get
            {
                yield return () => BooleanUnion.True;
                yield return () => BooleanUnion.False;
            }
        }

        public override IEnumerable<(BooleanUnion, BooleanUnion)> DifferentValues
        {
            get
            {
                yield return (BooleanUnion.True, BooleanUnion.False);
                yield return (BooleanUnion.False, BooleanUnion.True);
            }
        }

        public override BooleanUnion AnonymousValue => BooleanUnion.True;
    }
}
