using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests.Class
{
    public class MultipleCaseUnionWithNoParametersTests
    {
        [TestCase("Red")]
        [TestCase("Orange")]
        [TestCase("Green")]
        public void HasCaseMethod(string caseName)
        {
            var caseMethods = typeof(TrafficLights).GetFields(BindingFlags.Public | BindingFlags.Static)
                                                    .Where(m => m.Name == caseName)
                                                    .ToArray();
            //assert
            Assert.That(caseMethods, Has.Exactly(1).Items);
            var singleMethod = caseMethods[0];
            Assert.That(singleMethod, Has.Property(nameof(singleMethod.FieldType)).EqualTo(typeof(TrafficLights)));
        }
    }
}
