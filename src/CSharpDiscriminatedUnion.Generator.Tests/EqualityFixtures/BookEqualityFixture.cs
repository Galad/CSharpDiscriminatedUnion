using NUnit.Framework;
using CSharpDiscriminatedUnion.Generator.Tests.UnionTypes;
using System.Collections.Generic;
using System;

namespace CSharpDiscriminatedUnion.Generator.Tests.EqualityFixtures
{
    public class BookEqualityFixture : UnionEqualityFixture<Book>
    {
        private static Book e(string author, int pageCount, string title) => Book.NewBook(author, pageCount, title);

        public override IEnumerable<Func<Book>> SameValues
        {
            get
            {
                yield return () => e("a", 1, "c");
                yield return () => e("b", 1, "b");
                yield return () => e("", 0, "");
            }
        }

        public override IEnumerable<(Book, Book)> DifferentValues
        {
            get
            {
                yield return (e("a", 1, "c"), e("a", 1, "d"));
                yield return (e("a", 0, "c"), e("a", 1, "c"));
                yield return (e("b", 1, "c"), e("a", 1, "c"));
                yield return (e("a", 1, "c"), e(null, 0, null)); 
            }
        }

        public override Book AnonymousValue => e("zzz", 1, "xxx");        
    }
}