using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpDiscriminatedUnion.Attributes;

namespace CSharpDiscriminatedUnion.Generation.Tests.UnionTypes
{
    [GenerateDiscriminatedUnion]
    public partial struct StructBook
    {
        [StructCase("Book")]
        readonly string author;
        [StructCase("Book")]
        readonly int pageCount;
        [StructCase("Book")]
        readonly string title;
    }

    [GenerateDiscriminatedUnion]
    public partial struct MediaStruct
    {
        [StructCase("Book")]
        readonly string author;
        [StructCase("Book")]
        readonly int pages;
        [StructCase("Book")]
        readonly string bookTitle;
        [StructCase("Movie")]
        readonly string director;
        [StructCase("Movie")]
        readonly TimeSpan duration;
        [StructCase("Movie")]
        readonly string movieTitle;
        [StructCase("TvSeries")]
        readonly string tvSeriesTitle;
        [StructCase("TvSeries")]
        readonly int episodes;        
    }

    [GenerateDiscriminatedUnion]
    public partial struct StructShape
    {        
        [StructCase("Line")]
        partial class Cases
        {
        }

        [StructCase("Circle")]
        readonly double radius;
        [StructCase("Rectangle")]
        readonly double length;
        [StructCase("Rectangle")]
        readonly double width;
    }
    
    [GenerateDiscriminatedUnion]
    public partial struct StructShape_DefaultCircle
    {
        [StructCase("Line")]
        partial class Cases
        {
        }

        [StructCase("Circle", isDefaultValue: true)]
        readonly double radius;
        [StructCase("Rectangle")]
        readonly double length;
        [StructCase("Rectangle")]
        readonly double width;
    }

    [GenerateDiscriminatedUnion]
    public partial struct StructShape_DefaultRectangle1
    {
        [StructCase("Line")]
        partial class Cases
        {
        }

        [StructCase("Circle")]
        readonly double radius;
        [StructCase("Rectangle", isDefaultValue: true)]
        readonly double length;
        [StructCase("Rectangle")]
        readonly double width;
    }

    [GenerateDiscriminatedUnion]
    public partial struct StructShape_DefaultRectangle2
    {
        [StructCase("Line")]
        partial class Cases
        {
        }

        [StructCase("Circle")]
        readonly double radius;
        [StructCase("Rectangle")]
        readonly double length;
        [StructCase("Rectangle", isDefaultValue: true)]
        readonly double width;
    }
}
