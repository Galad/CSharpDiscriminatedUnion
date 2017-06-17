using NUnit.Framework;
using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using System.Collections.Generic;
using System;

namespace CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures
{
    public class EmailEqualityFixture : UnionEqualityFixture<Email>
    {
        public override IEnumerable<Func<Email>> SameValues
        {
            get
            {
                yield return () => e("a");
                yield return () => e("b");
                yield return () => e("");
            }
        }

        public override IEnumerable<(Email, Email)> DifferentValues
        {
            get
            {
                yield return (e("b"), e("a"));
                yield return (e("a"), e(null));
            }
        }

        public override Email AnonymousValue => e("email@outlook.com");
        private static Email e(string emailAddressValue) => Email.NewEmail(emailAddressValue);        
    }
}