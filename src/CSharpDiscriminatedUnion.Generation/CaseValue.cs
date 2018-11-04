using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Validation;

namespace CSharpDiscriminatedUnion.Generation
{
    internal class CaseValue
    {
        public FieldDeclarationSyntax Field { get; }
        public TypeSyntax Type => Field.Declaration.Type;
        public SyntaxToken Name => Field.Declaration.Variables[0].Identifier;
        public ITypeSymbol SymbolInfo { get; }
        public string Description { get; }

        /// <summary>Record Constructor</summary>
        /// <param name="field"><see cref="Field"/></param>
        public CaseValue(FieldDeclarationSyntax field, ITypeSymbol symbolInfo, string description = null)
        {
            Field = Requires.NotNull(field, nameof(field));
            SymbolInfo = Requires.NotNull(symbolInfo, nameof(symbolInfo));
            Description = description;
        }
    }
}
