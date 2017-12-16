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
            Requires.NotNull(member, nameof(member));
            return new DiscriminatedUnionContext<T>(
                _readonlyContext,
                GeneratedPartialClass,
                Members.Add(member),
                Cases
                );
        }

        public DiscriminatedUnionContext<T> AddMembers(IEnumerable<MemberDeclarationSyntax> members)
        {
            Requires.NotNull(members, nameof(members));
            return new DiscriminatedUnionContext<T>(
                _readonlyContext,
                GeneratedPartialClass,
                Members.AddRange(members),
                Cases
                );
        }

        public DiscriminatedUnionContext<T> WithCases(ImmutableArray<T> cases)
        {
            Requires.That(!cases.IsDefault, nameof(cases), "The parameter cannot be the default value");
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
                 Requires.NotNull(generatedPartialClass, nameof(generatedPartialClass)),
                Members,
                Cases
                );
        }
    }
}
