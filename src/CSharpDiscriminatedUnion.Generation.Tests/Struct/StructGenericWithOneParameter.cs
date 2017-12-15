using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests.Struct
{
    public class StructGenericWithOneParameter
    {
        [TestCase(0)]
        [TestCase("")]
        public void HasFactoryMethod<T>(T dummy)
        {
            var factoryMethod = typeof(IOStruct<T>).GetMethod("NewIO", BindingFlags.Public | BindingFlags.Static);
            //assert
            Assert.That(factoryMethod, Is.Not.Null);            
            Assert.That(factoryMethod.ReturnType, Is.EqualTo(typeof(IOStruct<T>)));
        }
    }
}
