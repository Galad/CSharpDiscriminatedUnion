using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;
using System;
using System.Collections.Generic;

namespace CSharpDiscriminatedUnion.Generator.Tests.EqualityFixtures
{
    public class TrafficLightsStructEqualityFixture : UnionEqualityFixture<TrafficLightsStruct>
    {
        public override IEnumerable<Func<TrafficLightsStruct>> SameValues
        {
            get
            {
                yield return () => TrafficLightsStruct.Red;
                yield return () => TrafficLightsStruct.Green;
                yield return () => TrafficLightsStruct.Orange;
            }
        }

        public override IEnumerable<(TrafficLightsStruct, TrafficLightsStruct)> DifferentValues
        {
            get
            {
                yield return (TrafficLightsStruct.Red, TrafficLightsStruct.Green);
                yield return (TrafficLightsStruct.Red, TrafficLightsStruct.Orange);
                yield return (TrafficLightsStruct.Green, TrafficLightsStruct.Red);
                yield return (TrafficLightsStruct.Green, TrafficLightsStruct.Orange);
                yield return (TrafficLightsStruct.Orange, TrafficLightsStruct.Red);
                yield return (TrafficLightsStruct.Orange, TrafficLightsStruct.Green);
            }
        }

        public override TrafficLightsStruct AnonymousValue => TrafficLightsStruct.Green;
    }

    public class StructShapeEqualityFixture : UnionEqualityFixture<StructShape>
    {
        public override IEnumerable<Func<StructShape>> SameValues
        {
            get
            {
                yield return () => StructShape.Line;
                yield return () => StructShape.NewCircle(2.0);
                yield return () => StructShape.NewCircle(10.0);
                yield return () => StructShape.NewRectangle(1.0, 1.0);
                yield return () => StructShape.NewRectangle(10.0, 10.0);
            }
        }

        public override IEnumerable<(StructShape, StructShape)> DifferentValues
        {
            get
            {
                yield return (StructShape.Line, StructShape.Line);
                yield return (StructShape.Line, StructShape.NewCircle(2.0));
                yield return (StructShape.Line, StructShape.NewRectangle(1.0, 1.0));
                yield return (StructShape.NewCircle(2.0), StructShape.Line);
                yield return (StructShape.NewCircle(2.0), StructShape.NewCircle(3.0));
                yield return (StructShape.NewCircle(2.0), StructShape.NewRectangle(1.0, 1.0));
                yield return (StructShape.NewRectangle(1.0, 2.0), StructShape.Line);
                yield return (StructShape.NewRectangle(1.0, 2.0), StructShape.NewCircle(3.0));
                yield return (StructShape.NewRectangle(1.0, 2.0), StructShape.NewRectangle(1.0, 1.0));
                yield return (StructShape.NewRectangle(1.0, 2.0), StructShape.NewRectangle(2.0, 2.0));
                yield return (StructShape.NewRectangle(1.0, 2.0), StructShape.NewRectangle(2.0, 1.0));                
            }
        }

        public override StructShape AnonymousValue => StructShape.NewCircle(1.0);
    }
}
