using System.Linq;
using Validation;

namespace CSharpDiscriminatedUnion.Generation
{
    internal class AggregateDiscriminatedUnionGenerator : IDiscriminatedUnionGenerator
    {
        private readonly IDiscriminatedUnionGenerator[] _innerGenerators;

        public AggregateDiscriminatedUnionGenerator(params IDiscriminatedUnionGenerator[] innerGenerators)
        {
            Requires.NotNull(innerGenerators, nameof(innerGenerators));
            _innerGenerators = innerGenerators;
        }

        public DiscriminatedUnionContext Build(DiscriminatedUnionContext context)
        {
            return _innerGenerators.Aggregate(context, (c, g) => g.Build(c));
        }
    }
}
