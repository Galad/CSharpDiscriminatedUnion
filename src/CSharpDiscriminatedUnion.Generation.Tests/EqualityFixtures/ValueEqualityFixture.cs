using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures
{
    public class ValueEqualityFixture : UnionEqualityFixture<Value>
    {
        public override IEnumerable<Func<Value>> SameValues
        {
            get
            {
                yield return () => Value.NewBoolean(true);
                yield return () => Value.NewBoolean(false);
                yield return () => Value.NewInteger(1);
                yield return () => Value.NewInteger(int.MaxValue);
                yield return () => Value.NewString("aaa");
                yield return () => Value.NewString("bbb");
                yield return () => Value.NewString("bbb");
                yield return () => Value.NewString(null);
            }
        }

        public override IEnumerable<(Value,Value)> DifferentValues
        {
            get
            {
                yield return (Value.NewBoolean(true), Value.NewBoolean(false));
                yield return (Value.NewBoolean(false), Value.NewBoolean(true));
                yield return (Value.NewInteger(0), Value.NewInteger(1));
                yield return (Value.NewInteger(int.MaxValue), Value.NewInteger(int.MinValue));
                yield return (Value.NewString("aaa"), Value.NewString("bbb"));
                yield return (Value.NewString("bbb"), Value.NewString("aaa"));
                yield return (Value.NewString(null), Value.NewString("yyy"));
                yield return (Value.NewString("zzz"), Value.NewString(null));
            }
        }

        public override Value AnonymousValue => Value.NewBoolean(true);        
    }
}
