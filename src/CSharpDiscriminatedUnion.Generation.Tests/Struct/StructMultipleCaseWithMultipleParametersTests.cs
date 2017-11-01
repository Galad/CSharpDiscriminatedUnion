using System;
using System.Linq;
using System.Reflection;
using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using NUnit.Framework;

namespace CSharpDiscriminatedUnion.Generation.Tests.Struct
{
    public class StructMultipleCaseWithMultipleParametersTests
    {
        [TestCase("Book", new[] { "author", "pages", "bookTitle" }, new[] { typeof(string), typeof(int), typeof(string) })]
        [TestCase("Movie", new[] { "director", "duration", "movieTitle" }, new[] { typeof(string), typeof(TimeSpan), typeof(string) })]
        [TestCase("TvSeries", new[] { "tvSeriesTitle", "episodes" }, new[] { typeof(string), typeof(int) })]
        public void HasCaseMethod(string caseName, string[] parameters, Type[] parameterTypes)
        {
            var caseMethods = typeof(MediaStruct).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                    .Where(m => m.Name == "New" + caseName)
                                                    .ToArray();
            //assert
            Assert.That(caseMethods, Has.Exactly(1).Items);
            var singleMethod = caseMethods[0];
            Assert.That(singleMethod, Has.Property(nameof(singleMethod.ReturnType)).EqualTo(typeof(MediaStruct)));
            Assert.That(singleMethod.GetParameters().Select(p => p.ParameterType), Is.EquivalentTo(parameterTypes));
            Assert.That(singleMethod.GetParameters().Select(p => p.Name), Is.EquivalentTo(parameters));
        }
    }
}
