using System.Linq;
using System.Reflection;
using NUnit.Framework;
using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;

namespace CSharpDiscriminatedUnion.Generator.Tests.Class
{
    public class SingleCaseWithParameterTests
    {
        [Test]
        public void HasCaseMethod()
        {
            var caseMethods = typeof(Email).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                    .Where(m => m.Name == "NewEmail")
                                                    .ToArray();
            //assert
            Assert.That(caseMethods, Has.Exactly(1).Items);
            var singleMethod = caseMethods[0];
            Assert.That(singleMethod, Has.Property(nameof(singleMethod.ReturnType)).EqualTo(typeof(Email)));
            Assert.That(singleMethod.GetParameters().Select(p => p.ParameterType), Is.EquivalentTo(new[] { typeof(string) }));
            Assert.That(singleMethod.GetParameters().Select(p => p.Name), Is.EquivalentTo(new[] { "emailAddress" }));
        }
    }
}