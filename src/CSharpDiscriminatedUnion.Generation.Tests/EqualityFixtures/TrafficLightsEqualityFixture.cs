using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures
{
    public class TrafficLightsEqualityFixture : UnionEqualityFixture<TrafficLights>
    {
        public override IEnumerable<Func<TrafficLights>> SameValues
        {
            get
            {
                yield return () => TrafficLights.Red;
                yield return () => TrafficLights.Green;
                yield return () => TrafficLights.Orange;
            }
        }

        public override IEnumerable<(TrafficLights, TrafficLights)> DifferentValues
        {
            get
            {
                yield return (TrafficLights.Red, TrafficLights.Green);
                yield return (TrafficLights.Red, TrafficLights.Orange);
                yield return (TrafficLights.Green, TrafficLights.Red);
                yield return (TrafficLights.Green, TrafficLights.Orange);
                yield return (TrafficLights.Orange, TrafficLights.Red);
                yield return (TrafficLights.Orange, TrafficLights.Green);
            }
        }

        public override TrafficLights AnonymousValue => TrafficLights.Green;        
    }

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
