using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using CSharpDiscriminatedUnion.Generation.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpDiscriminatedUnion.Generation
{
    internal struct DiscriminatedUnionContext<T> where T : IDiscriminatedUnionCase
    {
        //the context information that is only read
        private class ReadonlyContext
        {
            public TypeDeclarationSyntax UserDefinedClass;
            public SemanticModel SemanticModel;
            public INamedTypeSymbol SymbolInfo;
            public TypeSyntax Type;
            public SyntaxToken MatchGenericParameter;
        }

        public TypeDeclarationSyntax UserDefinedClass => _readonlyContext.UserDefinedClass;

        public DiscriminatedUnionContext<T> WithTypeParameterList(TypeParameterListSyntax typeParameterListSyntax)
        {
            return WithGeneratedPartialClass(
                Apply(c => c.WithTypeParameterList(typeParameterListSyntax), c => c.WithTypeParameterList(typeParameterListSyntax))
                );
        }

        public DiscriminatedUnionContext<T> AddAttributeLists(AttributeListSyntax attributeList)
        {
            return WithGeneratedPartialClass(
                Apply(c => c.AddAttributeLists(attributeList), c => c.AddAttributeLists(attributeList))
                );
        }

        public DiscriminatedUnionContext<T> WithMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            return WithGeneratedPartialClass(
               Apply(c => c.WithMembers(members), c => c.WithMembers(members))
               );
        }

        private TypeDeclarationSyntax Apply(
            Func<ClassDeclarationSyntax, TypeDeclarationSyntax> applyToClass,
            Func<StructDeclarationSyntax, TypeDeclarationSyntax> applyToStruct)
        {
            if (GeneratedPartialClass.Kind() == Microsoft.CodeAnalysis.CSharp.SyntaxKind.ClassDeclaration)
            {
                return applyToClass(GeneratedPartialClass as ClassDeclarationSyntax);
            }
            return applyToStruct(GeneratedPartialClass as StructDeclarationSyntax);
        }

        public SemanticModel SemanticModel => _readonlyContext.SemanticModel;
        public INamedTypeSymbol SymbolInfo => _readonlyContext.SymbolInfo;
        public SyntaxToken Name => UserDefinedClass.Identifier;
        public bool IsGeneric => SymbolInfo.IsGenericType;
        public TypeSyntax Type => _readonlyContext.Type;
        public SyntaxToken MatchGenericParameter => _readonlyContext.MatchGenericParameter;
        public ImmutableArray<T> Cases { get; }
        public ImmutableArray<MemberDeclarationSyntax> Members { get; }
        public TypeDeclarationSyntax GeneratedPartialClass { get; }
        public bool IsSingleCase => Cases.Length <= 1;
        
        private readonly ReadonlyContext _readonlyContext;

        public DiscriminatedUnionContext(
            TypeDeclarationSyntax userDefinedClass,
            TypeSyntax applyToClassType,
            TypeDeclarationSyntax generatedPartialClass,
            SemanticModel semanticModel,
            INamedTypeSymbol symbolInfo,
            ImmutableArray<T> cases)
        {
            _readonlyContext = new ReadonlyContext()
            {
                SymbolInfo = symbolInfo ?? throw new ArgumentNullException(nameof(symbolInfo)),
                SemanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel)),
                UserDefinedClass = userDefinedClass ?? throw new ArgumentNullException(nameof(userDefinedClass)),
                Type = applyToClassType ?? throw new ArgumentNullException(nameof(applyToClassType)),
                MatchGenericParameter = GeneratorHelpers.GenerateMatchResultGenericParameterName(userDefinedClass, symbolInfo.IsGenericType)
            };
            if (cases.IsDefault)
            {
                throw new ArgumentException("Cases cannot be a default value", nameof(cases));
            }
            GeneratedPartialClass = generatedPartialClass ?? throw new ArgumentNullException(nameof(generatedPartialClass));
            Members = ImmutableArray<MemberDeclarationSyntax>.Empty;
            Cases = cases;
        }

        private DiscriminatedUnionContext(
            ReadonlyContext readonlyContext,
            TypeDeclarationSyntax generatedPartialClass,
            ImmutableArray<MemberDeclarationSyntax> members,
            ImmutableArray<T> cases)
        {
            _readonlyContext = readonlyContext;
            Members = members;
            Cases = cases;
            GeneratedPartialClass = generatedPartialClass;
        }

        public DiscriminatedUnionContext<T> AddMember(MemberDeclarationSyntax member)
        {
            if (member is null)
            {
                throw new ArgumentNullException(nameof(member));
            }

            return new DiscriminatedUnionContext<T>(
                _readonlyContext,
                GeneratedPartialClass,
                Members.Add(member),
                Cases
                );
        }

        public DiscriminatedUnionContext<T> AddMembers(IEnumerable<MemberDeclarationSyntax> members)
        {
            if (members is null)
            {
                throw new ArgumentNullException(nameof(members));
            }

            return new DiscriminatedUnionContext<T>(
                _readonlyContext,
                GeneratedPartialClass,
                Members.AddRange(members),
                Cases
                );
        }

        public DiscriminatedUnionContext<T> WithCases(ImmutableArray<T> cases)
        {
            if (cases.IsDefault)
            {
                throw new ArgumentException("The parameter cannot be the default value", nameof(cases));
            }
            return new DiscriminatedUnionContext<T>(
                _readonlyContext,
                GeneratedPartialClass,
                Members,
                cases);
        }

        public DiscriminatedUnionContext<T> WithGeneratedPartialClass(TypeDeclarationSyntax generatedPartialClass)
        {
            return new DiscriminatedUnionContext<T>(
                _readonlyContext,
                 generatedPartialClass ?? throw new ArgumentNullException(nameof(generatedPartialClass)),
                Members,
                Cases
                );
        }
    }
}
