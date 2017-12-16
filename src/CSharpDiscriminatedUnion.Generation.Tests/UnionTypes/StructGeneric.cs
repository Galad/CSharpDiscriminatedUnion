using CSharpDiscriminatedUnion.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests.UnionTypes
{
    [GenerateDiscriminatedUnion]
    public partial struct IOStruct<T>
    {
        [StructCase("IO")]
        readonly T value;
    }

    [GenerateDiscriminatedUnion, System.Diagnostics.DebuggerDisplay("{DebugView}")]
    public partial struct EitherStruct<T>
    {
        [StructCase("Left")]
        readonly T valueLeft;
        [StructCase("Right")]
        readonly T valueRight;


        public string DebugView
        {
            get
            {
                switch (_tag)
                {
                    case Cases.Left:
                        return $"Left({valueLeft})";
                    case Cases.Right:
                        return $"Right({valueRight})";
                    default:
                        return "";
                }
            }
        }

        public override string ToString() => DebugView;
    }

    [GenerateDiscriminatedUnion, System.Diagnostics.DebuggerDisplay("{DebugView}")]
    public partial struct EitherStruct2<TLeft, TRight>
    {
        [StructCase("Left")]
        readonly TLeft valueLeft;
        [StructCase("Right")]
        readonly TRight valueRight;

        public string DebugView
        {
            get
            {
                switch (_tag)
                {
                    case Cases.Left:
                        var valueString = (valueLeft == null) ? "null" : valueLeft.ToString();
                        return $"Left({valueString})";
                    case Cases.Right:
                        var valueString2 = (valueRight == null) ? "null" : valueRight.ToString();
                        return $"Right({valueString2})";
                    default:
                        return "";
                }
            }
        }

        public override string ToString() => DebugView;
    }
}
