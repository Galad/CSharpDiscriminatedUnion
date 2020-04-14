using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generator.Tests.Struct
{
    public class StructGeneric
    {
        [TestCase(0)]
        [TestCase("")]
        public void IOStruct_HasFactoryMethod<T>(T dummy)
        {
            var factoryMethod = typeof(IOStruct<T>).GetMethod("NewIO", BindingFlags.Public | BindingFlags.Static);
            //assert
            Assert.That(factoryMethod, Is.Not.Null);            
            Assert.That(factoryMethod.ReturnType, Is.EqualTo(typeof(IOStruct<T>)));
        }

        [TestCase(0)]
        [TestCase("")]
        public void EitherStruct_HasFactoryMethod<T>(T dummy)
        {
            var factoryMethods = typeof(EitherStruct<T>).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                       .Where(m => m.Name.StartsWith("New"));
            //assert
            Assert.That(factoryMethods.Select(m => m.Name), Is.EquivalentTo(new[] { "NewLeft", "NewRight" }));
            Assert.That(factoryMethods.Select(m => m.ReturnType), Is.All.EqualTo(typeof(EitherStruct<T>)));
        }

        [TestCase(0, 0)]
        [TestCase(0, "")]
        [TestCase("", 0)]
        [TestCase("", "")]
        public void EitherStruct2_HasFactoryMethod<TLeft, TRight>(TLeft dummyLeft, TRight dummyRight)
        {
            var factoryMethods = typeof(EitherStruct2<TLeft, TRight>).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                       .Where(m => m.Name.StartsWith("New"));
            //assert
            Assert.That(factoryMethods.Select(m => m.Name), Is.EquivalentTo(new[] { "NewLeft", "NewRight" }));
            Assert.That(factoryMethods.Select(m => m.ReturnType), Is.All.EqualTo(typeof(EitherStruct2<TLeft, TRight>)));
        }
    }
}
