using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using System;
using System.Collections.Generic;

namespace CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures
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
}
