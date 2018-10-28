using CSharpDiscriminatedUnion.Attributes;
using CSharpDiscriminatedUnion.Generation.Tests.Idioms;
using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using NUnit.Framework;
using AutoFixture;
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

        [TestCaseSource(nameof(MatchMethodCases))]
        public void TestMatchMethod_HasDefaultCase(Type type)
        {
            var actual = type.GetMethods().Where(m =>
            {
                if (m.Name != "Match")
                {
                    return false;
                }
                var genericArgument = m.GetGenericArguments().Single();
                var noneFunc = typeof(Func<>).MakeGenericType(genericArgument);
                var parameters = m.GetParameters();
                return parameters.Any(p => p.Name == "none" && p.ParameterType == noneFunc) &&
                       parameters.Where(p => p.Name != "none")
                                 .All(p => p.IsOptional && p.DefaultValue == null);
            }).ToList();

            Assert.That(
                actual,
                Has.Count.EqualTo(1),
                "No Match method with default case function where found. The method must be generic, have a parameter 'Func<T> none', and all the other parameters must be optional."
                );
        }

        [TestCaseSource(nameof(MatchMethodCases))]
        public void TestMatchMethod_DefaultCaseReturnsCorrectValue(Type type)
        {
            var fixture = new Fixture();
            fixture.Register(() => ImmutableArray.Create(fixture.CreateMany<string>().ToArray()));
            var assertion = new MatchDefaultCaseMethodCanHandleAllCasesAssertion(fixture);
            assertion.Verify(type);
        }
    }
}
