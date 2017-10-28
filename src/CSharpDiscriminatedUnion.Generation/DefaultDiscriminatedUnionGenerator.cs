using CSharpDiscriminatedUnion.Generation.Generators;
using CSharpDiscriminatedUnion.Generation.Generators.Class;

namespace CSharpDiscriminatedUnion.Generation
{
    internal sealed class ClassDiscriminatedUnionGenerator : AggregateDiscriminatedUnionGenerator<DiscriminatedUnionCase>
    {
        public ClassDiscriminatedUnionGenerator(
            string factoryPrefix,
            bool preventNull)
            : base(
                  new ApplyGenericArguments<DiscriminatedUnionCase>(),
                  new GeneratePrivateConstructor<DiscriminatedUnionCase>(),
                  new CreateCasesPartialClassConstructor(),
                  new GenerateClassCasesFactoryMethods(factoryPrefix, preventNull),
                  new GenerateAbstractMatchMethod(),
                  new GenerateMatchImplementation(),
                  new GenerateAbstractEquatableImplementation<DiscriminatedUnionCase>(),
                  new GenerateCaseEquatableImplementation(),
                  new GenerateBaseEqualsOverride<DiscriminatedUnionCase>(),
                  new GenerateBaseEqualsOperatorOverload<DiscriminatedUnionCase>(),
                  new GenerateCaseGetHashCode(),
                  new AddGeneratedCodeAttribute<DiscriminatedUnionCase>("DiscriminitedUnion", "1.0"),
                  new GenerateBaseGetHashCodeImplementation<DiscriminatedUnionCase>()
                  )
        {
        }
    }
}
