using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests
{
    public class MultipleCaseUnionWithSingleParameterTests
    {
        [TestCase("Boolean", "boolValue", typeof(bool))]
        [TestCase("Integer", "intValue", typeof(int))]
        [TestCase("String", "stringValue", typeof(string))]
        public void HasCaseMethod(string caseName, string valueName, Type valueType)
        {
            var caseMethods = typeof(Value).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                    .Where(m => m.Name == "New" + caseName)
                                                    .ToArray();
            //assert
            Assert.That(caseMethods, Has.Exactly(1).Items);
            var singleMethod = caseMethods[0];
            Assert.That(singleMethod, Has.Property(nameof(singleMethod.ReturnType)).EqualTo(typeof(Value)));
            Assert.That(singleMethod.GetParameters().Select(p => p.ParameterType), Is.EquivalentTo(new[] { valueType }));
            Assert.That(singleMethod.GetParameters().Select(p => p.Name), Is.EquivalentTo(new[] { valueName }));
        }
    }
}
