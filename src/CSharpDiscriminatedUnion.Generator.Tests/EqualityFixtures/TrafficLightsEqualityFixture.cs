using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CSharpDiscriminatedUnion.Generator.Tests.EqualityFixtures
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
}
