using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using NUnit.Framework;
using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;

namespace CSharpDiscriminatedUnion.Generator.Tests.Class
{

    [TestFixture(typeof(NoCaseUnion))]
    [TestFixture(typeof(NoCaseUnionGeneric<object>))]
    [TestFixture(typeof(NoCaseUnionGeneric<object>))]
    [TestFixture(typeof(NoCaseUnionGeneric<int, string>))]
    [TestFixture(typeof(NoCaseUnionGeneric<int, string, object>))]
    [TestFixture(typeof(NoCaseUnionGenericWithConstraints<string, int, List<string>>))]
    public class NoCaseUnionTests<T>
    {
        [Test]
        public void HasNoMembers()
        {
            var members = typeof(T).GetMembers()
                                   .Where(m => m.MemberType != MemberTypes.Constructor &&
                                               (m.MemberType != MemberTypes.Method ||
                                                m.Name != "Equals" &&
                                                m.Name != "GetHashCode" &&
                                                m.Name != "GetType" &&
                                                m.Name != "ToString" &&
                                                m.Name != "op_Equality" &&
                                                m.Name != "op_Inequality"));
            //assert
            Assert.That(members, Is.Empty);
        }
    }
}
