using CSharpDiscriminatedUnion.Generation.Tests.UnionTypes;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests
{
    public class DebugViewTests
    {
        public static IEnumerable<TestCaseData> DebugViewCases
        {
            get
            {
                return new(object, string)[]
                {
                    (UnitStruct.Unit, "Unit()"),
                    (BooleanUnion.True, "True()"),
                    (BooleanUnion.False, "False()"),
                    (StructEmail.NewEmail("a"), "Email(a)"),
                    (StructEmail.NewEmail(""), "Email()"),
                    (StructEmail.NewEmail(null), "Email(null)"),
                    (StructBook.NewBook("a", 200, "b"), "Book(a, 200, b)"),
                    (MediaStruct.NewBook("a", 200, "b"), "Book(a, 200, b)"),
                    (MediaStruct.NewMovie("a", TimeSpan.Zero, "b"), "Movie(a, 00:00:00, b)"),
                    (StructShape.Line, "Line()"),
                    (StructShape.NewRectangle(1, 2.2), "Rectangle(1, 2.2)"),
                    (StructShape.NewRectangle(1, 2.2), "Rectangle(1, 2.2)"),

                    (SingleCaseUnion.Unit, "Unit()"),
                    (Email.NewEmail("a"), "Email(a)"),
                    (Email.NewEmail(""), "Email()"),
                    (Email.NewEmail(null), "Email(null)"),
                    (Age.NewAge(12), "Age(12)"),
                    (Book.NewBook("a", 200, "b"), "Book(a, 200, b)"),
                    (TrafficLights.Green, "Green()"),
                    (TrafficLights.Red, "Red()"),
                    (Value.NewBoolean(true), "Boolean(True)"),
                    (Value.NewInteger(12), "Integer(12)"),
                    (Value.NewString("a"), "String(a)"),
                    (Media.NewBook("a", 200, "b"), "Book(a, 200, b)"),
                    (Media.NewMovie("a", TimeSpan.Zero, "b"), "Movie(a, 00:00:00, b)"),
                }
                .Select(d => new TestCaseData(d.Item1, d.Item2));                             
            }
        }

        [TestCaseSource(nameof(DebugViewCases))]
        [SetCulture("en-US")]
        public void DebugView_ShouldReturnCorrectValue(object sut, string expected)
        {
            // arrange
            var type = sut.GetType();
            if (type.IsClass)
            {
                type = type.BaseType; // for classes, the property is private and declared in the base type
            }
            var property = type.GetProperty("DebugView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            // act
            var actual = (string)property.GetValue(sut);
            // assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}
