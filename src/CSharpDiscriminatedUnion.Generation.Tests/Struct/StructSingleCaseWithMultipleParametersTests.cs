using System.Linq;
using System.Reflection;
using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using NUnit.Framework;

namespace CSharpDiscriminatedUnion.Generation.Tests.Struct
{
    public class StructSingleCaseWithMultipleParametersTests
    {
        [Test]
        public void HasCaseMethod()
        {
            var caseMethods = typeof(StructBook).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                     .Where(m => m.Name == "NewBook")
                                                     .ToArray();
            //assert
            Assert.That(caseMethods, Has.Exactly(1).Items);
            var singleMethod = caseMethods[0];
            Assert.That(singleMethod, Has.Property(nameof(singleMethod.ReturnType)).EqualTo(typeof(StructBook)));
            Assert.That(singleMethod.GetParameters().Select(p => p.ParameterType), Is.EquivalentTo(new[] { typeof(string), typeof(int), typeof(string) }));
            Assert.That(singleMethod.GetParameters().Select(p => p.Name), Is.EquivalentTo(new[] { "author", "pageCount", "title" }));
        }
    }
}
