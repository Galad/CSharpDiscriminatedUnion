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
}
