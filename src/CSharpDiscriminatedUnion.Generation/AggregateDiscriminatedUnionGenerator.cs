using System.Linq;
using Validation;

namespace CSharpDiscriminatedUnion.Generation
{
    internal class AggregateDiscriminatedUnionGenerator<T> : IDiscriminatedUnionGenerator<T> where T : IDiscriminatedUnionCase
    {
        private readonly IDiscriminatedUnionGenerator<T>[] _innerGenerators;

        public AggregateDiscriminatedUnionGenerator(params IDiscriminatedUnionGenerator<T>[] innerGenerators)
        {
            Requires.NotNull(innerGenerators, nameof(innerGenerators));
            _innerGenerators = innerGenerators;
        }

        public DiscriminatedUnionContext<T> Build(DiscriminatedUnionContext<T> context)
        {
            return _innerGenerators.Aggregate(context, (c, g) => g.Build(c));
        }
    }
}
