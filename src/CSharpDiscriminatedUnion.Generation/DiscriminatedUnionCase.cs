using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Validation;

namespace CSharpDiscriminatedUnion.Generation
{
    internal struct DiscriminatedUnionCase : IDiscriminatedUnionCase
    {
        private class ReadonlyContext
        {
            public TypeDeclarationSyntax UserDefinedClass;
            public ClassDeclarationSyntax GeneratedPartialClass;
            public ImmutableArray<CaseValue> CaseValues;
            public int CaseNumber;
            public string Description;
        }

        public TypeDeclarationSyntax UserDefinedClass => _readonlyContext.UserDefinedClass;
        public ClassDeclarationSyntax GeneratedPartialClass => _readonlyContext.GeneratedPartialClass;
        public ImmutableArray<CaseValue> CaseValues => _readonlyContext.CaseValues;
        public int CaseNumber => _readonlyContext.CaseNumber;
        public SyntaxToken Name => _readonlyContext.UserDefinedClass.Identifier;
        public ImmutableArray<MemberDeclarationSyntax> Members { get; }
        public string Description => _readonlyContext.Description;        
        private readonly ReadonlyContext _readonlyContext;

        public DiscriminatedUnionCase(
            TypeDeclarationSyntax userDefinedClass,
            ClassDeclarationSyntax generatedPartialClass,
            ImmutableArray<CaseValue> caseValues,
            int caseNumber,
            string description = null)
        {
            Requires.That(!caseValues.IsDefault, nameof(caseValues), "Cases cannot be a default value");
            _readonlyContext = new ReadonlyContext()
            {
                UserDefinedClass = Requires.NotNull(userDefinedClass, nameof(userDefinedClass)),
                GeneratedPartialClass = Requires.NotNull(generatedPartialClass, nameof(generatedPartialClass)),
                CaseValues = caseValues,
                CaseNumber = caseNumber,
                Description = description,
            };
            Members = ImmutableArray<MemberDeclarationSyntax>.Empty;
        }

        private DiscriminatedUnionCase(
            ReadonlyContext readonlyContext,
            ImmutableArray<MemberDeclarationSyntax> members) : this()
        {
            _readonlyContext = readonlyContext;
            Members = members;

        }

        public DiscriminatedUnionCase AddMember(MemberDeclarationSyntax member)
        {
            Requires.NotNull(member, nameof(member));
            return new DiscriminatedUnionCase(
                _readonlyContext,
                Members.Add(member));
        }
    }
}
