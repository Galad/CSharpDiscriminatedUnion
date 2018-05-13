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
                return new(object, object)[]
                {
                    (default(TrafficLightsStruct), TrafficLightsStruct.Red),
                    (default(TrafficLightsStruct_DefaultGreen), TrafficLightsStruct_DefaultGreen.Green),
                    (default(TrafficLightsStruct_DefaultOrange), TrafficLightsStruct_DefaultOrange.Orange),
                    (default(TrafficLightsStruct_DefaultRed), TrafficLightsStruct_DefaultRed.Red),
                    (default(TrafficLightsStruct_MultipleDefault), TrafficLightsStruct_MultipleDefault.Green),
                    (default(StructShape), StructShape.Line),
                    (default(StructShape_DefaultCircle), StructShape_DefaultCircle.NewCircle(default)),
                    (default(StructShape_DefaultRectangle1), StructShape_DefaultRectangle1.NewRectangle(default, default)),
                    (default(StructShape_DefaultRectangle2), StructShape_DefaultRectangle2.NewRectangle(default, default))
                }
                .Select(t => new object[] { t.Item1, t.Item2 });
            }
        }

        [TestCaseSource(nameof(DefaultCaseTestCases))]
        public void DefaultValueOfStructUnion_WhenSettingDefaultValue_ShouldBeCorrectValue(object actual, object expected)
        {
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
