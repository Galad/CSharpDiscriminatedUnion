using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using NUnit.Framework;
using System.Linq;
using System.Reflection;

namespace CSharpDiscriminatedUnion.Generation.Tests.Struct
{
    public class StructMultipleCasesWithNoParametersTests
    {
        [TestCase("True")]
        [TestCase("False")]
        public void HasCaseSingletonValue(string expected)
        {
            var caseFields = typeof(BooleanUnion).GetFields(BindingFlags.Public | BindingFlags.Static)
                                                   .Where(f => f.IsInitOnly && f.Name == expected)
                                                   .ToArray();
            //assert
            Assert.That(caseFields, Has.Exactly(1).Items);
            Assert.That(caseFields[0].Name, Is.EqualTo(expected));
            Assert.That(caseFields[0].FieldType, Is.EqualTo(typeof(BooleanUnion)));
        }
    }
}
