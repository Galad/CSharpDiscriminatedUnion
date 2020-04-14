using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;
using NUnit.Framework;

namespace CSharpDiscriminatedUnion.Generator.Tests.Struct
{
    public class StructSingleCaseWithParameterTests
    {
        [Test]
        public void HasCaseMethod()
        {
            var caseMethods = typeof(StructEmail).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                    .Where(m => m.Name == "NewEmail")
                                                    .ToArray();
            //assert
            Assert.That(caseMethods, Has.Exactly(1).Items);
            var singleMethod = caseMethods[0];
            Assert.That(singleMethod, Has.Property(nameof(singleMethod.ReturnType)).EqualTo(typeof(StructEmail)));
            Assert.That(singleMethod.GetParameters().Select(p => p.ParameterType), Is.EquivalentTo(new[] { typeof(string) }));
            Assert.That(singleMethod.GetParameters().Select(p => p.Name), Is.EquivalentTo(new[] { "emailAddress" }));
        }
    }
}
