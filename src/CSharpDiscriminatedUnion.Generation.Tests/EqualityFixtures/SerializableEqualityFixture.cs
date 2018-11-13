using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests.EqualityFixtures
{
    public class SerializableEqualityFixture : UnionEqualityFixture<SerializableUnion>
    {
        private readonly IFormatter _formatter = new BinaryFormatter();

        private readonly MemoryStream _streamedDefault = new MemoryStream();
        private readonly MemoryStream _streamedOneValue = new MemoryStream();

        public SerializableEqualityFixture()
        {
            _formatter.Serialize(_streamedDefault, SerializableUnion.Default);
            _formatter.Serialize(_streamedOneValue, SerializableUnion.NewOneValue("foo"));
        }

        public override IEnumerable<Func<SerializableUnion>> SameValues
        {
            get
            {
                yield return () => Deserialize(_streamedDefault);
                yield return () => Deserialize(_streamedOneValue);
            }
        }

        public override IEnumerable<(SerializableUnion, SerializableUnion)> DifferentValues
        {
            get
            {
                yield return (Deserialize(_streamedDefault), Deserialize(_streamedOneValue));
            }
        }

        public override SerializableUnion AnonymousValue => SerializableUnion.Default;

        private SerializableUnion Deserialize(Stream stream)
        {
            stream.Seek(0L, SeekOrigin.Begin);
            return (SerializableUnion)_formatter.Deserialize(stream);
        }
    }
}
