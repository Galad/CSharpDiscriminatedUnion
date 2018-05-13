using System;
using System.Linq;
using System.Reflection;
using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using NUnit.Framework;

namespace CSharpDiscriminatedUnion.Generation.Tests.Struct
{
    public class StructMultipleCaseWithSingletonAndMultipleParametersTests
    {        
        [TestCase("Circle", new[] { "radius" }, new[] { typeof(double) })]
        [TestCase("Rectangle", new[] { "length", "width" }, new[] { typeof(double), typeof(double) })]
        public void HasCaseMethod(string caseName, string[] parameters, Type[] parameterTypes)
        {
            var caseMethods = typeof(StructShape).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                    .Where(m => m.Name == "New" + caseName)
                                                    .ToArray();
            //assert
            Assert.That(caseMethods, Has.Exactly(1).Items);
            var singleMethod = caseMethods[0];
            Assert.That(singleMethod, Has.Property(nameof(singleMethod.ReturnType)).EqualTo(typeof(StructShape)));
            Assert.That(singleMethod.GetParameters().Select(p => p.ParameterType), Is.EquivalentTo(parameterTypes));
            Assert.That(singleMethod.GetParameters().Select(p => p.Name), Is.EquivalentTo(parameters));
        }

        [TestCase("Line", new string[] { }, new Type[] { })]
        public void HasCaseFields(string caseName, string[] parameters, Type[] parameterTypes)
        {
            var caseFields = typeof(StructShape).GetFields(BindingFlags.Public | BindingFlags.Static)
                                                    .Where(m => m.Name == caseName)
                                                    .ToArray();
            //assert
            Assert.That(caseFields, Has.Exactly(1).Items);
            var field = caseFields[0];
            Assert.That(field, Has.Property(nameof(field.FieldType)).EqualTo(typeof(StructShape)));            
        }
    }
}
