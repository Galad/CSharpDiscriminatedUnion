using System.Reflection;
using NUnit.Framework;
using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using System.Collections.Generic;
using System;
using CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures;

namespace CSharpDiscriminatedUnion.Generation.Tests
{
    [TestFixture(typeof(Book), typeof(BookEqualityFixture))]
    [TestFixture(typeof(Email), typeof(EmailEqualityFixture))]
    [TestFixture(typeof(Age), typeof(AgeEqualityFixture))]
    [TestFixture(typeof(TrafficLights), typeof(TrafficLightsEqualityFixture))]
    [TestFixture(typeof(Value), typeof(ValueEqualityFixture))]
    [TestFixture(typeof(Employee), typeof(EmployeeEqualityFixture))]
    [TestFixture(typeof(Media), typeof(MediaEqualityFixture))]
    [TestFixture(typeof(Maybe<string>), typeof(MaybeEqualityReferenceFixture))]
    [TestFixture(typeof(Maybe<int>), typeof(MaybeEqualityValueFixture))]
    [TestFixture(typeof(Either<int, string>), typeof(EitherEqualityFixture))]
    [TestFixture(typeof(BooleanUnion), typeof(TrafficLightsStructEqualityFixture))]
    [TestFixture(typeof(TrafficLightsStruct), typeof(TrafficLightsStructEqualityFixture))]
    public class EqualityTests<T, TFixtureData>
        where TFixtureData : UnionEqualityFixture<T>, new()
        where T : class, IEquatable<T>
    {
        private static readonly TFixtureData Data = new TFixtureData();                
        public static IEnumerable<TestCaseData> EquatableEqualsTestCases => Data.EquatableEqualsTestCases;
        public static IEnumerable<TestCaseData> OperatorEqualityTestCases => Data.OperatorEqualityTestCases;
        public static IEnumerable<TestCaseData> OperatorInequalityTestCases => Data.OperatorInequalityTestCases;
        public static IEnumerable<TestCaseData> GetHashCodeSameValues => Data.GetHashCodeSameValues;
        public static IEnumerable<TestCaseData> GetHashCodeDifferentValues => Data.GetHashCodeDifferentValues;

        [TestCaseSource(nameof(EquatableEqualsTestCases))]
        public bool Equals_ShouldReturnCorrectValue(object actual, T other)
        {
            return actual.Equals(other);
        }

        [TestCase(3)]
        [TestCase("test")]
        [TestCase(null)]
        public void Equals_WithOtherType_ShouldReturnFalse(object other)
        {
            Assert.That(Data.AnonymousValue.Equals(other), Is.False);
        }


        [TestCaseSource(nameof(EquatableEqualsTestCases))]
        public bool EquatableEquals_ShouldReturnCorrectValue(IEquatable<T> actual, T other)
        {
            return actual.Equals(other);
        }

        private static MethodInfo EqualOperatorMethod = typeof(T).GetMethod("op_Equality", BindingFlags.Static | BindingFlags.Public);
        [TestCaseSource(nameof(OperatorEqualityTestCases))]
        public bool EqualOperator_ShouldReturnCorrectValue(T actual, T other)
        {
            return (bool)EqualOperatorMethod.Invoke(null, new[] { actual, other });
        }

        private static MethodInfo InequalOperatorMethod = typeof(T).GetMethod("op_Inequality", BindingFlags.Static | BindingFlags.Public);
        [TestCaseSource(nameof(OperatorInequalityTestCases))]
        public bool InequalOperator_ShouldReturnCorrectValue(T actual, T other)
        {
            return (bool)InequalOperatorMethod.Invoke(null, new[] { actual, other });
        }
        
        [TestCaseSource(nameof(GetHashCodeSameValues))]
        public void GetHashCode_WithSameValues_ReturnTheSameValue(T value, T otherValue)
        {
            var actual = value.GetHashCode();
            var other = otherValue.GetHashCode();
            Assert.That(actual, Is.EqualTo(other));
        }
        
        [TestCaseSource(nameof(GetHashCodeDifferentValues))]
        public void GetHashCode_WithDifferentValues_DoesNotReturnTheSameValue(
            T value,
            T otherValue)
        {
            var actual = value.GetHashCode();
            var other = otherValue.GetHashCode();
            Assert.That(actual, Is.Not.EqualTo(other));
        }
    }
}