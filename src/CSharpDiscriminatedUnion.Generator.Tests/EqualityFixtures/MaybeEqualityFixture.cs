using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generator.Tests.EqualityFixtures
{
    public class MaybeEqualityReferenceFixture : UnionEqualityFixture<Maybe<string>>
    {
        public override IEnumerable<Func<Maybe<string>>> SameValues
        {
            get
            {
                yield return () => Maybe<string>.None;
                yield return () => Maybe<string>.NewJust("red");
                yield return () => Maybe<string>.NewJust("green");
                yield return () => Maybe<string>.NewJust("");
                yield return () => Maybe<string>.NewJust(null);
            }
        }

        public override IEnumerable<(Maybe<string>, Maybe<string>)> DifferentValues
        {
            get
            {
                yield return (Maybe<string>.None, Maybe<string>.NewJust("red"));
                yield return (Maybe<string>.None, Maybe<string>.NewJust("green"));
                yield return (Maybe<string>.NewJust("red"), Maybe<string>.NewJust("green"));
                yield return (Maybe<string>.NewJust("red"), Maybe<string>.NewJust(""));
                yield return (Maybe<string>.NewJust("red"), Maybe<string>.NewJust(null));
            }
        }

        public override Maybe<string> AnonymousValue => Maybe<string>.NewJust("yellow");
    }

    public class MaybeEqualityValueFixture : UnionEqualityFixture<Maybe<int>>
    {
        public override IEnumerable<Func<Maybe<int>>> SameValues
        {
            get
            {
                yield return () => Maybe<int>.None;
                yield return () => Maybe<int>.NewJust(1);
                yield return () => Maybe<int>.NewJust(0);
                yield return () => Maybe<int>.NewJust(10);
                yield return () => Maybe<int>.NewJust(int.MaxValue);
                yield return () => Maybe<int>.NewJust(int.MinValue);
            }
        }

        public override IEnumerable<(Maybe<int>, Maybe<int>)> DifferentValues
        {
            get
            {
                yield return (Maybe<int>.None, Maybe<int>.NewJust(0));
                yield return (Maybe<int>.None, Maybe<int>.NewJust(10));
                yield return (Maybe<int>.NewJust(-1), Maybe<int>.NewJust(0));
                yield return (Maybe<int>.NewJust(int.MinValue), Maybe<int>.NewJust(0));
                yield return (Maybe<int>.NewJust(int.MaxValue), Maybe<int>.NewJust(10));
            }
        }

        public override Maybe<int> AnonymousValue => Maybe<int>.NewJust(99);
    }
}
