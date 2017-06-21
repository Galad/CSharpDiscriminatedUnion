using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;
using Validation;

namespace CSharpDiscriminatedUnion.Generation
{
    internal interface IDiscriminatedUnionCase
    {
        ImmutableArray<CaseValue> CaseValues { get; }
        int CaseNumber { get; }
        SyntaxToken Name { get; }
    }

    internal struct StructDiscriminatedUnionCase : IDiscriminatedUnionCase
    {
        private class ReadonlyContext
        {
            public SyntaxToken Name;   
            public ImmutableArray<CaseValue> CaseValues;
            public int CaseNumber;
        }
        
        public ImmutableArray<CaseValue> CaseValues => _readonlyContext.CaseValues;
        public int CaseNumber => _readonlyContext.CaseNumber;
        public SyntaxToken Name => _readonlyContext.Name;
        public ImmutableArray<MemberDeclarationSyntax> Members { get; }
        private readonly ReadonlyContext _readonlyContext;

        public StructDiscriminatedUnionCase(
            SyntaxToken name,
            ImmutableArray<CaseValue> caseValues,
            int caseNumber)
        {
            Requires.That(!caseValues.IsDefault, nameof(caseValues), "Cases cannot be a default value");
            _readonlyContext = new ReadonlyContext()
            {                
                CaseValues = caseValues,
                CaseNumber = caseNumber,
                Name = name
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

    internal struct ClassDiscriminatedUnionCase : IDiscriminatedUnionCase
    {
        private class ReadonlyContext
        {
            public TypeDeclarationSyntax UserDefinedClass;
            public ClassDeclarationSyntax GeneratedPartialClass;
            public ImmutableArray<CaseValue> CaseValues;
            public int CaseNumber;
        }

        public TypeDeclarationSyntax UserDefinedClass => _readonlyContext.UserDefinedClass;
        public ClassDeclarationSyntax GeneratedPartialClass => _readonlyContext.GeneratedPartialClass;
        public ImmutableArray<CaseValue> CaseValues => _readonlyContext.CaseValues;
        public int CaseNumber => _readonlyContext.CaseNumber;
        public SyntaxToken Name => _readonlyContext.UserDefinedClass.Identifier;
        public ImmutableArray<MemberDeclarationSyntax> Members { get; }
        private readonly ReadonlyContext _readonlyContext;

        public ClassDiscriminatedUnionCase(
            TypeDeclarationSyntax userDefinedClass,
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

        private ClassDiscriminatedUnionCase(
            ReadonlyContext readonlyContext,
            ImmutableArray<MemberDeclarationSyntax> members) : this()
        {
            _readonlyContext = readonlyContext;
            Members = members;

        }

        public ClassDiscriminatedUnionCase AddMember(MemberDeclarationSyntax member)
        {
            Requires.NotNull(member, nameof(member));
            return new ClassDiscriminatedUnionCase(
                _readonlyContext,
                Members.Add(member));
        }
    }
}
