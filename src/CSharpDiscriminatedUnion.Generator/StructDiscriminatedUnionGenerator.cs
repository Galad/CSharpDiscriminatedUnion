using CSharpDiscriminatedUnion.Generator.Generators;
using CSharpDiscriminatedUnion.Generator.Generators.Struct;

namespace CSharpDiscriminatedUnion.Generator
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
                  new GenerateStructGetHashCode(),
                  new GenerateStructMatchMethod(),
                  new GenerateMatchDefaultCaseMethod<StructDiscriminatedUnionCase>(),
                  new GenerateDebugView<StructDiscriminatedUnionCase>()
                  )
        {            
        }
    }
}
