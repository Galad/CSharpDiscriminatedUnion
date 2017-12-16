using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests.Struct
{
    public class StructSingleCaseTests
    {
        [Test]
        public void HasCaseSingletonValue()
        {
            var caseFields = typeof(UnitStruct).GetFields(BindingFlags.Public | BindingFlags.Static)
                                                   .Where(f => f.IsInitOnly)
                                                   .ToArray();
            //assert
            Assert.That(caseFields, Has.Exactly(1).Items);
            Assert.That(caseFields[0].Name, Is.EqualTo("Unit"));
            Assert.That(caseFields[0].FieldType, Is.EqualTo(typeof(UnitStruct)));
        }

        [Test]
        public void SingleCase_EqualsShouldReturnCorrectValue()
        {
            var actual = UnitStruct.Unit;
            Assert.That(actual, Is.EqualTo(UnitStruct.Unit));
        }
    }
}
