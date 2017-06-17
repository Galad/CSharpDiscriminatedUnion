using CSharpDiscriminatedUnion.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generation.Tests.UnionTypes
{
    [GenerateDiscriminatedUnion]
    public partial class TrafficLights
    {
        partial class Cases
        {
            partial class Red : TrafficLights { }
            partial class Orange : TrafficLights { }
            partial class Green : TrafficLights { }
        }
    }

    [GenerateDiscriminatedUnion]
    public partial class Value
    {
        partial class Cases
        {
            partial class Boolean : Value
            {
                readonly bool boolValue;
            }
            partial class Integer : Value
            {
                readonly int intValue;
            }
            partial class String : Value
            {
                readonly string stringValue;
            }
        }
    }

    [GenerateDiscriminatedUnion]
    public partial class Employee
    {
        partial class Cases
        {
            partial class Manager : Employee
            {
                readonly string name;
            }
            partial class Engineer : Employee
            {
                readonly string name;
            }
            partial class TestEngineer : Employee
            {
                readonly string name;
            }
        }
    }

    [GenerateDiscriminatedUnion]
    public partial class Media
    {
        partial class Cases
        {
            partial class Book : Media
            {
                readonly string author;
                readonly int pages;
                readonly string title;
            }
            partial class Movie : Media
            {
                readonly string director;
                readonly TimeSpan duration;
                readonly string title;
            }
            partial class TvSeries : Media
            {
                readonly string title;
                readonly int episodes;
            }
        }
    }
}
