using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;

namespace CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures
{
    public class StructShapeFixture : UnionEqualityFixture<StructShape>
    {
        public override IEnumerable<Func<StructShape>> SameValues
        {
            get
            {
                yield return () => StructShape.Line;
                yield return () => StructShape.NewCircle(1.0);
                yield return () => StructShape.NewCircle(10.0);
                yield return () => StructShape.NewRectangle(1.0, 2.0);
                yield return () => StructShape.NewRectangle(10.0, 20.0);
            }
        }

        public override IEnumerable<(StructShape, StructShape)> DifferentValues
        {
            get
            {
                yield return (StructShape.Line, StructShape.NewCircle(1.0));
                yield return (StructShape.Line, StructShape.NewRectangle(1.0, 2.0));
                yield return (StructShape.NewCircle(2.0), StructShape.Line);
                yield return (StructShape.NewCircle(2.0), StructShape.NewRectangle(10.0, 20.0));
                yield return (StructShape.NewRectangle(1.0, 2.0), StructShape.Line);
                yield return (StructShape.NewRectangle(1.0, 2.0), StructShape.NewCircle(1.0));
                yield return (StructShape.NewRectangle(1.0, 2.0), StructShape.NewRectangle(2.0, 1.0));
            }
        }

        public override StructShape AnonymousValue => StructShape.NewCircle(99.0);
    }
}
