using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using NUnit.Framework;
using System;
using System.Linq;
using System.Reflection;

namespace CSharpDiscriminatedUnion.Generation.Tests
{
    public class MultipleCaseUnionWithSingleOfSameTypeParameterTests
    {
        [TestCase("Manager")]
        [TestCase("Engineer")]
        [TestCase("TestEngineer")]
        public void HasCaseMethod(string caseName)
        {
            var caseMethods = typeof(Employee).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                    .Where(m => m.Name == "New" + caseName)
                                                    .ToArray();
            //assert
            Assert.That(caseMethods, Has.Exactly(1).Items);
            var singleMethod = caseMethods[0];
            Assert.That(singleMethod, Has.Property(nameof(singleMethod.ReturnType)).EqualTo(typeof(Employee)));
            Assert.That(singleMethod.GetParameters().Select(p => p.ParameterType), Is.EquivalentTo(new[] { typeof(string) }));
            Assert.That(singleMethod.GetParameters().Select(p => p.Name), Is.EquivalentTo(new[] { "name" }));
        }
    }

    public class MultipleCaseUnionWithMultipleParametersTests
    {
        [TestCase("Book", new[] { "author", "pages", "title" }, new[] { typeof(string), typeof(int), typeof(string) })]
        [TestCase("Movie", new[] { "director", "duration", "title" }, new[] { typeof(string), typeof(TimeSpan), typeof(string) })]
        [TestCase("TvSeries", new[] { "title", "episodes" }, new[] { typeof(string), typeof(int) })]
        public void HasCaseMethod(string caseName, string[] parameters, Type[] parameterTypes)
        {
            var caseMethods = typeof(Media).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                    .Where(m => m.Name == "New" + caseName)
                                                    .ToArray();
            //assert
            Assert.That(caseMethods, Has.Exactly(1).Items);
            var singleMethod = caseMethods[0];
            Assert.That(singleMethod, Has.Property(nameof(singleMethod.ReturnType)).EqualTo(typeof(Media)));
            Assert.That(singleMethod.GetParameters().Select(p => p.ParameterType), Is.EquivalentTo(parameterTypes));
            Assert.That(singleMethod.GetParameters().Select(p => p.Name), Is.EquivalentTo(parameters));
        }
    }
}
