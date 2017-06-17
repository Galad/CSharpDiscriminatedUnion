using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Validation;

namespace CSharpDiscriminatedUnion.Generation
{
    internal struct DiscriminatedUnionCase
    {
        private class ReadonlyContext
        {
            public ClassDeclarationSyntax UserDefinedClass;
            public ClassDeclarationSyntax GeneratedPartialClass;
            public ImmutableArray<CaseValue> CaseValues;
            public int CaseNumber;
        }

        public ClassDeclarationSyntax UserDefinedClass => _readonlyContext.UserDefinedClass;
        public ClassDeclarationSyntax GeneratedPartialClass => _readonlyContext.GeneratedPartialClass;
        public ImmutableArray<CaseValue> CaseValues => _readonlyContext.CaseValues;
        public int CaseNumber => _readonlyContext.CaseNumber;
        public SyntaxToken Name => _readonlyContext.UserDefinedClass.Identifier;
        public ImmutableArray<MemberDeclarationSyntax> Members { get; }
        private readonly ReadonlyContext _readonlyContext;
        
        public DiscriminatedUnionCase(
            ClassDeclarationSyntax userDefinedClass,
            ClassDeclarationSyntax generatedPartialClass,
            ImmutableArray<CaseValue> caseValues,
            int caseNumber)
        {
            Requires.That(!caseValues.IsDefault, nameof(caseValues), "Cases cannot be a default value");
            _readonlyContext = new ReadonlyContext()
            {
                UserDefinedClass = Requires.NotNull(userDefinedClass, nameof(userDefinedClass)),
                GeneratedPartialClass = Requires.NotNull(generatedPartialClass, nameof(generatedPartialClass)),
                CaseValues = caseValues,
                CaseNumber = caseNumber
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
