using CSharpDiscriminatedUnion.Attributes;
using CSharpDiscriminatedUnion.Generation.Tests.Idioms;
using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using NUnit.Framework;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests
{
    public class MatchMethodTests
    {
        public static IEnumerable<Type> MatchMethodCases
        {
            get
            {
                return typeof(MatchMethodTests)
                    .Assembly
                    .GetTypes()
                    .Where(t => t.IsPublic &&
                                t != typeof(NoCaseUnion) &&
                                t != typeof(NoCaseUnionGeneric<>) &&
                                t != typeof(NoCaseUnionGeneric<,>) &&
                                t != typeof(NoCaseUnionGeneric<,,>) &&
                                t != typeof(NoCaseUnionGenericWithConstraints<,,>) && 
                                t != typeof(PreventNull5<>) &&
                                t.Namespace == typeof(NoCaseUnion).Namespace
                          );
            }
        }

        public static IEnumerable<TestCaseData> MatchMethodCasesClass =>
            MatchMethodCases.Where(t => t.IsClass).Select(t => new TestCaseData(t));

        public static IEnumerable<TestCaseData> MatchMethodCasesStruct =>
            MatchMethodCases.Where(t => t.IsValueType).Select(t => new TestCaseData(t));

        [TestCaseSource(nameof(MatchMethodCasesClass))]
        public void TestMatchMethod_Class(Type type)
        {
            TestMatchMethod(type);
        }

        [TestCaseSource(nameof(MatchMethodCasesStruct))]
        public void TestMatchMethod_Struct(Type type)
        {
            TestMatchMethod(type);
        }

        private static void TestMatchMethod(Type type)
        {
            var fixture = new Fixture();
            fixture.Register(() => ImmutableArray.Create(fixture.CreateMany<string>().ToArray()));
            var assertion = new MatchMethodCanHandleAllCasesAssertion(fixture);
            assertion.Verify(type);
        }
    }
}
