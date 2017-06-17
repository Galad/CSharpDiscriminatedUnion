using CodeGeneration.Roslyn;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace CSharpDiscriminatedUnion.Attributes
{
    /// <summary>
    /// Generates a discriminated union from a C# template.
    /// The C# template is a partial class containing a nested class for each union case
    /// </summary>
    /// <example>
    /// <![CDATA[
    /// [GenerateDiscriminatedUnion]
    /// public partial class Maybe<T>
    /// {
    ///     static partial class Cases
    ///     {
    ///         partial class None : Maybe<T> { }
    ///         
    ///         partial class Just : Maybe<T>
    ///         {
    ///             readonly T value;
    ///         }
    ///     }
    /// }
    /// ]]>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    [CodeGenerationAttribute("CSharpDiscriminatedUnion.Generation.CodeGenerator, CSharpDiscriminatedUnion.Generation, Version=" + ThisAssembly.AssemblyVersion + ", Culture=neutral, PublicKeyToken=" + ThisAssembly.PublicKeyToken)]
    [Conditional("CodeGeneration")]
    public class GenerateDiscriminatedUnionAttribute : Attribute
    {        
    }
}
