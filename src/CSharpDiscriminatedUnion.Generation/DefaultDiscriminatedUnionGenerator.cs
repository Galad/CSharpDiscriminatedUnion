using CSharpDiscriminatedUnion.Generation.Generators;

namespace CSharpDiscriminatedUnion.Generation
{
    internal sealed class DefaultDiscriminatedUnionGenerator : AggregateDiscriminatedUnionGenerator
    {
        public DefaultDiscriminatedUnionGenerator()
            : base(
                  new ApplyGenericArguments(),
                  new GeneratePrivateConstructor(),
                  new CreateCasesPartialClassConstructor(),
                  new GenerateCaseFactoryMethods(),
                  new GenerateAbstractMatchMethod(),
                  new GenerateMatchImplementation(),
                  new GenerateAbstractEquatableImplementation(),
                  new GenerateCaseEquatableImplementation(),
                  new GenerateBaseEqualsOverride(),
                  new GenerateBaseEqualsOperatorOverload(),
                  new GenerateCaseGetHashCode(),
                  new AddGeneratedCodeAttribute("DiscriminitedUnion", "1.0"),
                  new GenerateBaseGetHashCodeImplementation()
                  )
        {
        }
    }
}
