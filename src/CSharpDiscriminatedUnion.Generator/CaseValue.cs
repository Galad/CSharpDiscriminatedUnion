using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CSharpDiscriminatedUnion.Generator
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
            Field = field ?? throw new System.ArgumentNullException(nameof(field));
            SymbolInfo = symbolInfo ?? throw new System.ArgumentNullException(nameof(symbolInfo));
            Description = description;
        }
    }
}
