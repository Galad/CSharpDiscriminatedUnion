using CSharpDiscriminatedUnion.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests.UnionTypes
{
    [Serializable]
    [GenerateDiscriminatedUnion]
    public partial class SerializableUnion
    {
        partial class Cases
        {
            [Serializable]
            partial class Default : SerializableUnion
            {
            }

            [Serializable]
            partial class OneValue : SerializableUnion
            {
                readonly string stringValue;
            }
        }
    }
}
