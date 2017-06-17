using CSharpDiscriminatedUnion.Attributes;

namespace CSharpDiscriminatedUnion.Generation.Tests.UnionTypes
{
    [GenerateDiscriminatedUnion]
    public partial class SingleCaseUnion
    {
        static partial class Cases
        {
            partial class Unit : SingleCaseUnion
            {
            }
        }
    }

    [GenerateDiscriminatedUnion]
    public partial class Email
    {
        static partial class Cases
        {
            partial class Email : UnionTypes.Email
            {
                readonly string emailAddress;
            }
        }
    }

    [GenerateDiscriminatedUnion]
    public partial class Age
    {
        static partial class Cases
        {
            partial class Age : UnionTypes.Age
            {
                readonly int age;
            }
        }
    }

    [GenerateDiscriminatedUnion]
    public partial class Book
    {
        static partial class Cases
        {
            partial class Book : UnionTypes.Book
            {
                readonly string author;
                readonly int pageCount;
                readonly string title;
            }
        }
    }
}