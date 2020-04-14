using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;
using System;
using System.CodeDom.Compiler;

namespace CSharpDiscriminatedUnion.Generator.Tests.Class
{
    [TestFixture(typeof(NoCaseUnion))]
    [TestFixture(typeof(NoCaseUnionGeneric<object>))]
    [TestFixture(typeof(NoCaseUnionGeneric<object>))]
    [TestFixture(typeof(NoCaseUnionGeneric<int, string>))]
    [TestFixture(typeof(NoCaseUnionGeneric<int, string, object>))]
    [TestFixture(typeof(NoCaseUnionGenericWithConstraints<string, int, List<string>>))]
    [TestFixture(typeof(SingleCaseUnion))]
    [TestFixture(typeof(Age))]
    [TestFixture(typeof(Email))]
    [TestFixture(typeof(Book))]
    [TestFixture(typeof(Value))]
    [TestFixture(typeof(TrafficLights))]
    [TestFixture(typeof(Employee))]
    [TestFixture(typeof(Media))]
    [TestFixture(typeof(Maybe<string>))]
    [TestFixture(typeof(Maybe<int>))]
    [TestFixture(typeof(Maybe<Maybe<Maybe<int>>>))]
    [TestFixture(typeof(Either<int, int>))]
    [TestFixture(typeof(Either<int, double>))]
    [TestFixture(typeof(Either<int, string>))]
    [TestFixture(typeof(PreventNull1))]
    [TestFixture(typeof(PreventNull2))]
    [TestFixture(typeof(PreventNull3<string>))]
    public class CommonPropertiesTests<T>
    {        
        private static IEnumerable<TestCaseData> TestSource
        {
            get
            {
                yield return new TestCaseData().SetName("{m}<" + typeof(T).FormatGenericTypeName() + ">{a}");
            }
        }

        [TestCaseSource(nameof(TestSource))]
        public void HasOnlyPrivateConstructors()
        {
            var constructors = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.NonPublic)
                .Where(c => !c.IsPrivate)
                .ToArray();
            //assert            
            Assert.That(constructors, Is.Empty);
        }

        [TestCaseSource(nameof(TestSource))]
        public void IsAbstract()
        {
            //assert
            Assert.That(typeof(T).IsAbstract, Is.True);
        }

        [TestCaseSource(nameof(TestSource))]
        public void ImplementIEquatable()
        {
            //assert
            Assert.That(typeof(IEquatable<T>).IsAssignableFrom(typeof(T)), Is.True);
        }

        [TestCaseSource(nameof(TestSource))]
        public void HasGeneratedCodeAttribute()
        {
            //assert
            Assert.IsNotNull(typeof(T).GetCustomAttribute<GeneratedCodeAttribute>());
        }
    }
}
