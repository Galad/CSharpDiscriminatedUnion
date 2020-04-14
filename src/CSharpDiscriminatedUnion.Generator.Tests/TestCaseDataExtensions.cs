using NUnit.Framework;

namespace CSharpDiscriminatedUnion.Generator.Tests
{
    public static class TestCaseDataExtensions
    {
        public static TestCaseData SetName<T>(this TestCaseData testCaseData) => testCaseData.SetName("{m}<" + typeof(T).FormatGenericTypeName() + ">{a}");
    }
}