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
    public class OptionFactoryPrefixTests
    {
        [TestCase("Create", typeof(OptionFactoryPrefix1))]
        [TestCase("New", typeof(OptionFactoryPrefix2))]
        [TestCase("", typeof(OptionFactoryPrefix3))]
        [TestCase("", typeof(OptionFactoryPrefix4))]
        public void HasCaseMethodWithCorrectPrefix(string prefix, Type type)
        {
            var caseMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                    .Where(m => m.Name == prefix + "Case1")
                                                    .ToArray();            
            //assert
            Assert.That(caseMethods, Has.Exactly(1).Items);
            var singleMethod = caseMethods[0];
            Assert.That(singleMethod, Has.Property(nameof(singleMethod.ReturnType)).EqualTo(type));
            Assert.That(singleMethod.GetParameters().Select(p => p.ParameterType), Is.EquivalentTo(new[] { typeof(int) }));
            Assert.That(singleMethod.GetParameters().Select(p => p.Name), Is.EquivalentTo(new[] { "value" }));
        }
    }
}
