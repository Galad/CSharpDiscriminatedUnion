using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using NUnit.Framework;

namespace CSharpDiscriminatedUnion.Generation.Tests.Struct
{
    public class DefaultCaseTest
    {
        public static IEnumerable<object[]> DefaultCaseTestCases
        {
            get
            {
                yield return new object[] { default(TrafficLightsStruct), TrafficLightsStruct.Red };
                yield return new object[] { default(TrafficLightsStruct_DefaultGreen), TrafficLightsStruct_DefaultGreen.Green };
                yield return new object[] { default(TrafficLightsStruct_DefaultOrange), TrafficLightsStruct_DefaultOrange.Orange };
                yield return new object[] { default(TrafficLightsStruct_DefaultRed), TrafficLightsStruct_DefaultRed.Red };
                yield return new object[] { default(TrafficLightsStruct_MultipleDefault), TrafficLightsStruct_MultipleDefault.Green };
            }
        }

        [TestCaseSource(nameof(DefaultCaseTestCases))]
        public void DefaultValueOfStructUnion_WhenSettingDefaultValue_ShouldBeCorrectValue(object actual, object expected)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
