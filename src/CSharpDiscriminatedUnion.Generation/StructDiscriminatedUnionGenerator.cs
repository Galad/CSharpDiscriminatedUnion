using CSharpDiscriminatedUnion.Generation.Generators;
using CSharpDiscriminatedUnion.Generation.Generators.Struct;

namespace CSharpDiscriminatedUnion.Generation
{
    internal sealed class StructDiscriminatedUnionGenerator : AggregateDiscriminatedUnionGenerator<StructDiscriminatedUnionCase>
    {
        public StructDiscriminatedUnionGenerator(
            string factoryPrefix,
            bool preventNull)
            : base(
                  new ApplyGenericArguments<StructDiscriminatedUnionCase>(),
                  new GenerateStructCases(),
                  new GenerateTagField<StructDiscriminatedUnionCase>(),
                  new GenerateStructConstructor(),
                  new GenerateStructCasesFactoryMethods(factoryPrefix, preventNull),
                  new GenerateStructEquatable(),
                  new GenerateBaseEqualsOperatorOverload<StructDiscriminatedUnionCase>(),
                  new GenerateStructEqualsOverride(),
                  new GenerateStructGetHashCode()
                  )
        {            
        }
    }
}
