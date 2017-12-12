using System.Linq;
using System.Reflection;
using NUnit.Framework;
using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;

namespace CSharpDiscriminatedUnion.Generation.Tests.Class
{
    public class SingleCaseTests
    {
        [Test]
        public void HasCaseSingletonValue()
        {
            var caseFields = typeof(SingleCaseUnion).GetFields(BindingFlags.Public | BindingFlags.Static)
                                                   .Where(f => f.IsInitOnly)
                                                   .ToArray();
            //assert
            Assert.That(caseFields, Has.Exactly(1).Items);
            Assert.That(caseFields[0].Name, Is.EqualTo("Unit"));
            Assert.That(caseFields[0].FieldType, Is.EqualTo(typeof(SingleCaseUnion)));
        }

        [Test]
        public void SingleCase_EqualsShouldReturnCorrectValue()
        {
            var actual = SingleCaseUnion.Unit;
            Assert.That(actual, Is.EqualTo(SingleCaseUnion.Unit));
        }
    }
}