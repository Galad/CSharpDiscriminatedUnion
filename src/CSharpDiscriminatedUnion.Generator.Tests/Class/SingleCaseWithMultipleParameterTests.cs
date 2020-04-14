using System.Linq;
using System.Reflection;
using NUnit.Framework;
using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;

namespace CSharpDiscriminatedUnion.Generator.Tests.Class
{

    public class SingleCaseWithMultipleParameterTests
    {
        [Test]
        public void HasCaseMethod()
        {
            var caseMethods = typeof(Book).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                    .Where(m => m.Name == "NewBook")
                                                    .ToArray();
            //assert
            Assert.That(caseMethods, Has.Exactly(1).Items);
            var singleMethod = caseMethods[0];
            Assert.That(singleMethod, Has.Property(nameof(singleMethod.ReturnType)).EqualTo(typeof(Book)));
            Assert.That(singleMethod.GetParameters().Select(p => p.ParameterType), Is.EquivalentTo(new[] { typeof(string), typeof(int), typeof(string) }));
            Assert.That(singleMethod.GetParameters().Select(p => p.Name), Is.EquivalentTo(new[] { "author", "pageCount", "title" }));
        }        
    }
}