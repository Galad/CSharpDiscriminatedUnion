using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Validation;

namespace CSharpDiscriminatedUnion.Generation
{
    internal struct StructDiscriminatedUnionCase : IDiscriminatedUnionCase
    {
        private class ReadonlyContext
        {
            public SyntaxToken Name;
            public ImmutableArray<CaseValue> CaseValues;
            public int CaseNumber;
            public string Description;
        }

        public ImmutableArray<CaseValue> CaseValues => _readonlyContext.CaseValues;
        public int CaseNumber => _readonlyContext.CaseNumber;
        public SyntaxToken Name => _readonlyContext.Name;
        public ImmutableArray<MemberDeclarationSyntax> Members { get; }
        public string Description => _readonlyContext.Description;
        private readonly ReadonlyContext _readonlyContext;

        public StructDiscriminatedUnionCase(
            SyntaxToken name,
            ImmutableArray<CaseValue> caseValues,
            int caseNumber,
            string description = null)
        {
            Requires.That(!caseValues.IsDefault, nameof(caseValues), "Cases cannot be a default value");
            _readonlyContext = new ReadonlyContext()
            {
                CaseValues = caseValues,
                CaseNumber = caseNumber,
                Name = name,
                Description = description
            };
            Members = ImmutableArray<MemberDeclarationSyntax>.Empty;
        }

        private StructDiscriminatedUnionCase(
            ReadonlyContext readonlyContext,
            ImmutableArray<MemberDeclarationSyntax> members) : this()
        {
            _readonlyContext = readonlyContext;
            Members = members;
        }

        public StructDiscriminatedUnionCase AddMember(MemberDeclarationSyntax member)
        {
            Requires.NotNull(member, nameof(member));
            return new StructDiscriminatedUnionCase(
                _readonlyContext,
                Members.Add(member));
        }
    }
}
