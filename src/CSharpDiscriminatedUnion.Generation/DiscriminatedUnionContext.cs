using Microsoft.CodeAnalysis;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Validation;
using CSharpDiscriminatedUnion.Generation.Generators;

namespace CSharpDiscriminatedUnion.Generation
{
    internal struct DiscriminatedUnionContext
    {
        //the context information that is only read
        private class ReadonlyContext
        {
            public ClassDeclarationSyntax UserDefinedClass;
            public SemanticModel SemanticModel;
            public INamedTypeSymbol SymbolInfo;
            public TypeSyntax Type;
            public SyntaxToken MatchGenericParameter;
        }

        public ClassDeclarationSyntax UserDefinedClass => _readonlyContext.UserDefinedClass;
        public SemanticModel SemanticModel => _readonlyContext.SemanticModel;
        public INamedTypeSymbol SymbolInfo => _readonlyContext.SymbolInfo;
        public SyntaxToken Name => UserDefinedClass.Identifier;
        public bool IsGeneric => SymbolInfo.IsGenericType;
        public TypeSyntax Type => _readonlyContext.Type;
        public SyntaxToken MatchGenericParameter => _readonlyContext.MatchGenericParameter;
        public ImmutableArray<DiscriminatedUnionCase> Cases { get; }
        public ImmutableArray<MemberDeclarationSyntax> Members { get; }
        public ClassDeclarationSyntax GeneratedPartialClass { get; }

        private readonly ReadonlyContext _readonlyContext;

        public DiscriminatedUnionContext(
            ClassDeclarationSyntax userDefinedClass,
            TypeSyntax applyToClassType,
            ClassDeclarationSyntax generatedPartialClass,
            SemanticModel semanticModel,
            INamedTypeSymbol symbolInfo,
            ImmutableArray<DiscriminatedUnionCase> cases)
        {
            _readonlyContext = new ReadonlyContext()
            {
                SymbolInfo = Requires.NotNull(symbolInfo, nameof(symbolInfo)),
                SemanticModel = Requires.NotNull(semanticModel, nameof(semanticModel)),
                UserDefinedClass = Requires.NotNull(userDefinedClass, nameof(userDefinedClass)),
                Type = Requires.NotNull(applyToClassType, nameof(applyToClassType)),
                MatchGenericParameter = GeneratorHelpers.GenerateMatchResultGenericParameterName(userDefinedClass, symbolInfo.IsGenericType)
            };
            Requires.That(!cases.IsDefault, nameof(cases), "Cases cannot be a default value");
            GeneratedPartialClass = Requires.NotNull(generatedPartialClass, nameof(generatedPartialClass));
            Members = ImmutableArray<MemberDeclarationSyntax>.Empty;
            Cases = cases;
        }

        private DiscriminatedUnionContext(
            ReadonlyContext readonlyContext,
            ClassDeclarationSyntax generatedPartialClass,
            ImmutableArray<MemberDeclarationSyntax> members,
            ImmutableArray<DiscriminatedUnionCase> cases)
        {
            _readonlyContext = readonlyContext;
            Members = members;
            Cases = cases;
            GeneratedPartialClass = generatedPartialClass;
        }

        public DiscriminatedUnionContext AddMember(MemberDeclarationSyntax member)
        {
            Requires.NotNull(member, nameof(member));
            return new DiscriminatedUnionContext(
                _readonlyContext,
                GeneratedPartialClass,
                Members.Add(member),
                Cases
                );
        }

        public DiscriminatedUnionContext AddMembers(IEnumerable<MemberDeclarationSyntax> members)
        {
            Requires.NotNull(members, nameof(members));
            return new DiscriminatedUnionContext(
                _readonlyContext,
                GeneratedPartialClass,
                Members.AddRange(members),
                Cases
                );
        }

        public DiscriminatedUnionContext WithCases(ImmutableArray<DiscriminatedUnionCase> cases)
        {
            Requires.That(!cases.IsDefault, nameof(cases), "The parameter cannot be the default value");
            return new DiscriminatedUnionContext(
                _readonlyContext,
                GeneratedPartialClass,
                Members,
                cases);
        }

        public DiscriminatedUnionContext WithGeneratedPartialClass(ClassDeclarationSyntax generatedPartialClass)
        {
            return new DiscriminatedUnionContext(
                _readonlyContext,
                 Requires.NotNull(generatedPartialClass, nameof(generatedPartialClass)),
                Members,
                Cases
                );
        }
    }
}
