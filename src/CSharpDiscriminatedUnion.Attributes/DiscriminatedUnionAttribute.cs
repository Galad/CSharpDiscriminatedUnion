﻿using CodeGeneration.Roslyn;
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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false, AllowMultiple = true)]
    [CodeGenerationAttribute("CSharpDiscriminatedUnion.Generator.CodeGenerator, CSharpDiscriminatedUnion.Generator, Version=" + ThisAssembly.AssemblyVersion + ", Culture=neutral, PublicKeyToken=" + ThisAssembly.PublicKeyToken)]
    [Conditional("CodeGeneration")]
    public class GenerateDiscriminatedUnionAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the prefix for the case factory methods.
        /// The default value is <c>New</c>
        /// </summary>
        public string CaseFactoryPrefix { get; set; } = "New";
        /// <summary>
        /// Throws a <see cref="ArgumentNullException"/> when trying to create a case with null values. 
        /// Example: Maybe&lt;string&gt;.NewSome(null) will throw an exception
        /// </summary>
        public bool PreventNullValues { get; set; }
    }

    /// <summary>
    /// Add valueless named case to a discriminated union. Use it on the partial class Cases.
    /// </summary>
    /// <example>
    /// [GenerateDiscriminatedUnion]    
    /// public partial struct BooleanUnion
    /// {
    ///     [StructCase("True")]
    ///     [StructCase("False", true)]
    ///     static partial class Cases
    ///     {
    ///     }
    /// }
    /// </example>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class StructCaseAttribute : Attribute
    {
        /// <summary>
        /// The case name, which will be used to generate code
        /// </summary>
        public string CaseName { get; }
        /// <summary>
        /// Specify that the case if the default value for the current type
        /// </summary>
        public bool IsDefaultValue { get; }
        /// <summary>
        /// A description of the case. The description is used in in the XML documentation of the generated code
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Creates a new instance of <see cref="StructCaseAttribute"/>
        /// </summary>
        /// <param name="caseName">The case name, which will be used to generate code</param>
        /// <param name="isDefaultValue">Specify that the case if the default value for the current type</param>
        /// <param name="description">A description of the case. The description is used in in the XML documentation of the generated code</param>
        public StructCaseAttribute(string caseName, bool isDefaultValue = false, string description = null)
        {
            if (string.IsNullOrEmpty(caseName))
            {
                throw new ArgumentNullException(nameof(caseName));
            }
            CaseName = caseName;
            IsDefaultValue = isDefaultValue;
            Description = description;
        }
    }
}
