using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace CSharpDiscriminatedUnion.Generator
{
    internal interface IDiscriminatedUnionCase
    {
        ImmutableArray<CaseValue> CaseValues { get; }
        int CaseNumber { get; }
        SyntaxToken Name { get; }
        string Description { get; }
    }
}
