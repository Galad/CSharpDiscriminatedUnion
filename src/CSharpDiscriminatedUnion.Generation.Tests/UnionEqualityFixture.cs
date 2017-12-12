using System.Linq;
using NUnit.Framework;
using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using System.Collections.Generic;
using System;

namespace CSharpDiscriminatedUnion.Generation.Tests
{
    public abstract class UnionEqualityFixture<T>
    {
        private IEnumerable<TestCaseData> NullTestCase => new[] { new TestCaseData(AnonymousValue, default(T)).Returns(false) };
        public abstract IEnumerable<Func<T>> SameValues { get; }
        public abstract IEnumerable<(T, T)> DifferentValues { get; }
        public IEnumerable<TestCaseData> EqualityTestCasesSameValues =>
            SameValues.Select(f => new TestCaseData(f(), f()).Returns(true).SetName<T>());
        public IEnumerable<TestCaseData> EqualityTestCasesDifferentValues
            => DifferentValues.Select(v => new TestCaseData(v.Item1, v.Item2).Returns(false).SetName<T>());
        public IEnumerable<TestCaseData> EquatableEqualsTestCases => 
            EqualityTestCasesSameValues.Concat(EqualityTestCasesDifferentValues)
                                       .Concat(NullTestCase);
        public IEnumerable<TestCaseData> OperatorEqualityTestCases
        {
            get
            {
                return EqualityTestCasesSameValues.Concat(
                    EqualityTestCasesDifferentValues)
                    .Concat(
                        EqualityTestCasesDifferentValues.Select(c => new TestCaseData(c.Arguments[1], c.Arguments[0]).Returns(c.ExpectedResult).SetName<T>())
                        )
                    .Concat(NullTestCase)
                    .Concat(new[]
                    {
                        new TestCaseData(default(T), default(T)).Returns(true).SetName<T>(),
                        new TestCaseData(default(T), AnonymousValue).Returns(false).SetName<T>(),
                    });
            }
        }
        public IEnumerable<TestCaseData> OperatorInequalityTestCases
        {
            get
            {
                return OperatorEqualityTestCases.Select(c => new TestCaseData(c.Arguments).Returns(!(bool)c.ExpectedResult).SetName<T>());
            }
        }

        public IEnumerable<TestCaseData> GetHashCodeSameValues
        {
            get
            {
                return SameValues.Select(d => new TestCaseData(d(), d()).SetName<T>());
            }
        }

        public IEnumerable<TestCaseData> GetHashCodeDifferentValues
        {
            get
            {
                return DifferentValues.Select(d => new TestCaseData(d.Item1, d.Item2).SetName<T>());
            }
        }

        public abstract T AnonymousValue { get; }
    }
}