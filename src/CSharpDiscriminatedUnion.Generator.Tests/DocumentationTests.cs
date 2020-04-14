using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NUnit.Framework;

namespace CSharpDiscriminatedUnion.Generator.Tests
{
    public class DocumentationTests : CompilationTestsBase
    {
        [Test]
        [TestCase(DocOnCaseAndParameters, DocOnCaseAndParametersExpected, TestName = nameof(DocOnCaseAndParameters))]
        [TestCase(DocOnCase, DocOnCaseExpected, TestName = nameof(DocOnCase))]
        [TestCase(NoDoc, NoDocExpected, TestName = nameof(NoDoc))]
        [TestCase(DocOnCaseWithoutParameters, DocOnCaseWithoutParametersExpected, TestName = nameof(DocOnCaseWithoutParameters))]
        [TestCase(NoDocStruct, NoDocStructExpected, TestName = nameof(NoDocStruct))]
        [TestCase(DocOnCaseWithoutParametersStruct, DocOnCaseWithoutParametersStructExpected, TestName = nameof(DocOnCaseWithoutParametersStruct))]
        [TestCase(DocOnCaseStruct, DocOnCaseStructExpected, TestName = nameof(DocOnCaseStruct))]
        [TestCase(DocOnCaseAndParametersStruct, DocOnCaseAndParametersStructExpected, TestName = nameof(DocOnCaseAndParametersStruct))]
        public async Task GeneratedCodeGenerateDocumentationForFactoryMethods(string source, string expected)
        {
            // act
            var result = await Generate(source);
            var actual = result.GetText().ToString();
            
            // assert
            var normalizedActual = NormalizeForComparison(actual);
            var normalizedExpected = NormalizeForComparison(expected);
            Assert.That(normalizedActual, Contains.Substring(normalizedExpected), actual);
        }

        private string NormalizeForComparison(string value)
        {
            return Regex.Replace(
                value.Replace("\r", string.Empty)
                     .Replace("\n", string.Empty),
                "\\s",
                string.Empty
                );
        }

        #region DocOnCaseAndParameters
        const string DocOnCaseAndParameters = @"
using System;
using CSharpDiscriminatedUnion.Attributes;

namespace Types
{
    [GenerateDiscriminatedUnion]
    partial class Test
    {
        partial class Cases
        {
            /// <summary>
            /// Represents something
            /// More details
            /// </summary>
            partial class Case1 : Test
            {
                /// <summary>
                /// Some value
                /// </summary>
                readonly string value;
                /// <summary>
                /// Some other value
                /// </summary>
                readonly string value2;
            }
        }
    }
}";
        const string DocOnCaseAndParametersExpected = @"
/// <summary>
/// Creates a Case1
/// Represents something
/// More details
/// </summary>
/// <param name=""value"">Some value</param>
/// <param name=""value2"">Some other value</param>
public static Types.Test NewCase1(string value, string value2)
";
        #endregion

        #region DocOnCase
        const string DocOnCase = @"
using System;
using CSharpDiscriminatedUnion.Attributes;

namespace Types
{
    [GenerateDiscriminatedUnion]
    partial class Test
    {
        partial class Cases
        {
            /// <summary>
            /// Represents something
            /// More details
            /// </summary>
            partial class Case1 : Test
            {
                readonly string value;
                readonly string value2;
            }
        }
    }
}";
        const string DocOnCaseExpected = @"
/// <summary>
/// Creates a Case1
/// Represents something
/// More details
/// </summary>
/// <param name=""value""></param>
/// <param name=""value2""></param>
public static Types.Test NewCase1(string value, string value2)
";
        #endregion

        #region NoDoc
        const string NoDoc = @"
using System;
using CSharpDiscriminatedUnion.Attributes;

namespace Types
{
    [GenerateDiscriminatedUnion]
    partial class Test
    {
        partial class Cases
        {
            partial class Case1 : Test
            {
                readonly string value;
                readonly string value2;
            }
        }
    }
}";
        const string NoDocExpected = @"
/// <summary>
/// Creates a Case1
/// </summary>
/// <param name=""value""></param>
/// <param name=""value2""></param>
public static Types.Test NewCase1(string value, string value2)
";
        #endregion

        #region DocOnCaseWithoutParameters
        const string DocOnCaseWithoutParameters = @"
using System;
using CSharpDiscriminatedUnion.Attributes;

namespace Types
{
    [GenerateDiscriminatedUnion]
    partial class Test
    {
        partial class Cases
        {
            /// <summary>
            /// Represents something
            /// More details
            /// </summary>
            partial class Case1 : Test
            {
            }
        }
    }
}";
        const string DocOnCaseWithoutParametersExpected = @"
/// <summary>
/// Creates a Case1
/// Represents something
/// More details
/// </summary>
public static readonly Types.Test Case1 = new Cases.Case1();
";
        #endregion

        #region NoDocStruct
        const string NoDocStruct = @"
using System;
using CSharpDiscriminatedUnion.Attributes;

namespace Types
{
    [GenerateDiscriminatedUnion]
    partial struct Test
    {
        [StructCase(""Case1"")]
        readonly string value;
        [StructCase(""Case1"")]
        readonly string value2;
    }
}";
        const string NoDocStructExpected = @"
/// <summary>
/// Creates a Case1
/// </summary>
/// <param name=""value""></param>
/// <param name=""value2""></param>
public static Types.Test NewCase1(string value, string value2)
";
        #endregion

        #region DocOnCaseWithoutParametersStruct
        const string DocOnCaseWithoutParametersStruct = @"
using System;
using CSharpDiscriminatedUnion.Attributes;

namespace Types
{
    [GenerateDiscriminatedUnion]
    partial struct Test
    {
        [StructCase(""Case1"", description: ""Represents something"")]
        partial class Cases
        {
        }
    }
}";
        const string DocOnCaseWithoutParametersStructExpected = @"
/// <summary>
/// Creates a Case1
/// Represents something
/// </summary>
public static readonly Types.Test Case1 = new Types.Test();
";
        #endregion

        #region DocOnCaseStruct
        const string DocOnCaseStruct = @"
using System;
using CSharpDiscriminatedUnion.Attributes;

namespace Types
{
    [GenerateDiscriminatedUnion]
    partial struct Test
    {
        [StructCase(""Case1"", description: ""Represents something"")]
        readonly string value;
        [StructCase(""Case1"", description: ""Represents something else"")]
        readonly string value2;
    }
}";
        const string DocOnCaseStructExpected = @"
/// <summary>
/// Creates a Case1
/// Represents something
/// </summary>
/// <param name=""value""></param>
/// <param name=""value2""></param>
public static Types.Test NewCase1(string value, string value2)
";
        #endregion

        #region DocOnCaseAndParametersStruct
        const string DocOnCaseAndParametersStruct = @"
using System;
using CSharpDiscriminatedUnion.Attributes;

namespace Types
{
    [GenerateDiscriminatedUnion]
    partial struct Test
    {
        /// <summary>
        /// Some value
        /// </summary>
        [StructCase(""Case1"", description: ""Represents something"")]
        readonly string value;
        /// <summary>
        /// Some other value
        /// </summary>
        [StructCase(""Case1"", description: ""Represents something else"")]
        readonly string value2;
    }
}";
        const string DocOnCaseAndParametersStructExpected = @"
/// <summary>
/// Creates a Case1
/// Represents something
/// </summary>
/// <param name=""value"">Some value</param>
/// <param name=""value2"">Some other value</param>
public static Types.Test NewCase1(string value, string value2)
";
        #endregion
    }
}
