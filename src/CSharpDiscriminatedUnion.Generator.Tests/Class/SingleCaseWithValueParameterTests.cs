using System.Linq;
using System.Reflection;
using NUnit.Framework;
using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;
using System.Collections.Generic;
using System;

namespace CSharpDiscriminatedUnion.Generator.Tests.Class
{
    public class SingleCaseWithValueParameterTests
    {
        [Test]
        public void HasCaseMethod()
        {
            var caseMethods = typeof(Age).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                         .Where(m => m.Name == "NewAge")
                                         .ToArray();
            //assert
            Assert.That(caseMethods, Has.Exactly(1).Items);
            var singleMethod = caseMethods[0];
            Assert.That(singleMethod, Has.Property(nameof(singleMethod.ReturnType)).EqualTo(typeof(Age)));
            Assert.That(singleMethod.GetParameters().Select(p => p.ParameterType), Is.EquivalentTo(new[] { typeof(int) }));
            Assert.That(singleMethod.GetParameters().Select(p => p.Name), Is.EquivalentTo(new[] { "age" }));
        }      
    }
}