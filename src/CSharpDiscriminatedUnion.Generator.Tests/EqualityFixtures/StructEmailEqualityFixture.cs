using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;
using System.Collections.Generic;
using System;

namespace CSharpDiscriminatedUnion.Generator.Tests.EqualityFixtures
{
    public class StructEmailEqualityFixture : UnionEqualityFixture<StructEmail>
    {
        public override IEnumerable<Func<StructEmail>> SameValues
        {
            get
            {
                yield return () => e("a");
                yield return () => e("b");
                yield return () => e("");
            }
        }

        public override IEnumerable<(StructEmail, StructEmail)> DifferentValues
        {
            get
            {
                yield return (e("b"), e("a"));
                yield return (e("a"), e(null));
            }
        }

        public override StructEmail AnonymousValue => e("email@outlook.com");
        private static StructEmail e(string emailAddressValue) => StructEmail.NewEmail(emailAddressValue);
    }
}