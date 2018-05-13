using CSharpDiscriminatedUnion.Attributes;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CSharpDiscriminatedUnion.Generation.Tests.UnionTypes
{
    [GenerateDiscriminatedUnion]
    public partial struct UnitStruct
    {
        [StructCase("Unit")]        
        static partial class Cases
        {
        }
    }

    [GenerateDiscriminatedUnion]    
    public partial struct BooleanUnion
    {
        [StructCase("True")]
        [StructCase("False")]
        static partial class Cases
        {
        }
    }

    [GenerateDiscriminatedUnion]
    public partial struct TrafficLightsStruct
    {
        [StructCase("Red")]
        [StructCase("Orange")]
        [StructCase("Green")]        
        static partial class Cases
        {
        }
    }

    [GenerateDiscriminatedUnion]
    public partial struct StructEmail
    {
        [StructCase("Email")]
        readonly string emailAddress;
    }


    [GenerateDiscriminatedUnion]
    public partial struct TrafficLightsStruct_DefaultRed
    {
        [StructCase("Red", true)]
        [StructCase("Orange")]
        [StructCase("Green")]
        static partial class Cases
        {
        }
    }

    [GenerateDiscriminatedUnion]
    public partial struct TrafficLightsStruct_DefaultOrange
    {
        [StructCase("Red")]
        [StructCase("Orange", true)]
        [StructCase("Green")]
        static partial class Cases
        {
        }
    }

    [GenerateDiscriminatedUnion]
    public partial struct TrafficLightsStruct_DefaultGreen
    {
        [StructCase("Red")]
        [StructCase("Orange")]
        [StructCase("Green", true)]
        static partial class Cases
        {
        }
    }
    
    [GenerateDiscriminatedUnion]
    public partial struct TrafficLightsStruct_MultipleDefault
    {
        [StructCase("Red")]
        [StructCase("Green", true)]
        [StructCase("Green2", true)]
        [StructCase("Orange")]        
        static partial class Cases
        {
        }
    }
}
