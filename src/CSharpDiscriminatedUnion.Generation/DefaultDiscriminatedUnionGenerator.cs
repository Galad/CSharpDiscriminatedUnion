using CSharpDiscriminatedUnion.Generation.Generators;
using CSharpDiscriminatedUnion.Generation.Generators.Struct;

namespace CSharpDiscriminatedUnion.Generation
{
    internal sealed class ClassDiscriminatedUnionGenerator : AggregateDiscriminatedUnionGenerator<ClassDiscriminatedUnionCase>
    {
        public ClassDiscriminatedUnionGenerator(
            string factoryPrefix,
            bool preventNull)
            : base(
                  new ApplyGenericArguments<ClassDiscriminatedUnionCase>(),
                  new GeneratePrivateConstructor<ClassDiscriminatedUnionCase>(),
                  new CreateCasesPartialClassConstructor(),
                  new GenerateClassCasesFactoryMethods(factoryPrefix, preventNull),
                  new GenerateAbstractMatchMethod(),
                  new GenerateMatchImplementation(),
                  new GenerateAbstractEquatableImplementation<ClassDiscriminatedUnionCase>(),
                  new GenerateCaseEquatableImplementation(),
                  new GenerateBaseEqualsOverride<ClassDiscriminatedUnionCase>(),
                  new GenerateBaseEqualsOperatorOverload<ClassDiscriminatedUnionCase>(),
                  new GenerateCaseGetHashCode(),
                  new AddGeneratedCodeAttribute<ClassDiscriminatedUnionCase>("DiscriminitedUnion", "1.0"),
                  new GenerateBaseGetHashCodeImplementation<ClassDiscriminatedUnionCase>()
                  )
        {
        }
    }

    internal sealed class StructDiscriminatedUnionGenerator : AggregateDiscriminatedUnionGenerator<StructDiscriminatedUnionCase>
    {
        public StructDiscriminatedUnionGenerator(
            string factoryPrefix,
            bool preventNull)
            : base(
                  new GenerateStructCases(),
                  new GenerateTagField<StructDiscriminatedUnionCase>(),
                  new GenerateStructConstructor(),
                  new GenerateStructCasesFactoryMethods(factoryPrefix, preventNull)
                  )
        {            
        }
    }
}
