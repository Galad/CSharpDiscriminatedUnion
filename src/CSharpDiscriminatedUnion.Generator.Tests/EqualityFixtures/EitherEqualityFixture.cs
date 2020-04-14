using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generator.Tests.EqualityFixtures
{
    public class EitherEqualityFixture : UnionEqualityFixture<Either<int, string>>
    {
        private static Either<int, string> Left(int left) => Either<int, string>.NewLeft(left);
        private static Either<int, string> Right(string right) => Either<int, string>.NewRight(right);

        public override IEnumerable<Func<Either<int, string>>> SameValues
        {
            get
            {
                yield return () => Left(0);
                yield return () => Left(1);
                yield return () => Left(2);
                yield return () => Right("red");
                yield return () => Right("yellow");
                yield return () => Right("");
                yield return () => Right(null);
            }
        }

        public override IEnumerable<(Either<int, string>, Either<int, string>)> DifferentValues
        {
            get
            {
                yield return (Left(1), Left(2));
                yield return (Left(1), Left(0));
                yield return (Left(1), Left(int.MaxValue));
                yield return (Right("red"), Left(0));
                yield return (Right("red"), Left(1));
                yield return (Right("red"), Right("green"));
                yield return (Right("red"), Right("yellow"));
                yield return (Right("red"), Right(""));
                yield return (Right("red"), Right(null));                
            }
        }

        public override Either<int, string> AnonymousValue => Left(99);
    }
}
