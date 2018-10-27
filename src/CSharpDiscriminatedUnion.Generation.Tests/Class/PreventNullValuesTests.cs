using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using NUnit.Framework;
using AutoFixture;
using AutoFixture.Idioms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests
{
    public class PreventNullValuesTests
    {
        [Test]
        public void CaseWithOneValue_VerifyGuardClauses()
        {
            Test<PreventNull1>();
        }

        [Test]
        public void CaseWithMultipleValues_VerifyGuardClauses()
        {
            Test<PreventNull2>();
        }

        [Test]
        public void CaseWithGenericUsingReferenceType_VerifyGuardClauses()
        {
            Test<PreventNull3<string>>();
        }

        [Test]
        public void CaseWithGenericUsingValueType_VerifyGuardClauses()
        {
            Test<PreventNull3<int>>();
        }

        [Test]
        public void CaseWithConstrainedGenericUsingValueType_VerifyGuardClauses()
        {
            Test<PreventNull4<int>>();
        }

        [Test]
        public void CaseWithConstrainedGenericUsingReferenceType_VerifyGuardClauses()
        {
            Test<PreventNull5<string>>();
        }

        private static void Test<T>()
        {
            var assertion = new GuardClauseAssertion(new Fixture());
            assertion.Verify(GetFactoryMethods<T>());
        }

        private static IEnumerable<MethodInfo> GetFactoryMethods<T>()
        {
            return typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(m => m.Name.StartsWith("New"));
        }
    }
}
