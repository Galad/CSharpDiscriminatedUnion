using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;

namespace CSharpDiscriminatedUnion.Generator
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
            if (caseValues.IsDefault)
            {
                throw new ArgumentException("Cases cannot be a default value", nameof(caseValues));
            }
            _readonlyContext = new ReadonlyContext()
            {
                UserDefinedClass = userDefinedClass ?? throw new ArgumentNullException(nameof(userDefinedClass)),
                GeneratedPartialClass = generatedPartialClass ?? throw new ArgumentNullException(nameof(generatedPartialClass)),
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
            if (member is null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            return new DiscriminatedUnionCase(
                _readonlyContext,
                Members.Add(member));
        }
    }
}
