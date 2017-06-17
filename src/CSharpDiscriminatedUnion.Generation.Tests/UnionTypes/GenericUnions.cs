using CSharpDiscriminatedUnion.Attributes;
using System;
using System.Diagnostics.CodeAnalysis;

namespace CSharpDiscriminatedUnion.Generation.Tests.UnionTypes
{
    [GenerateDiscriminatedUnion]
    public partial class Maybe<T>
    {
        static partial class Cases
        {
            partial class None : Maybe<T> { }
            partial class Just : Maybe<T>
            {
                readonly T value;
            }
        }
    }

    // Just test different with the compilation
    // We want to check if there is not collision between the user defined generic parameters, 
    // And the result generic parameters of the Match methods
    [GenerateDiscriminatedUnion, ExcludeFromCodeCoverage]
    partial class Maybe2<TResult, TResult2, TResult3, TArg, TArg1, Targ2>
    {
        static partial class Cases
        {            
            partial class Case : Maybe2<TResult, TResult2, TResult3, TArg, TArg1, Targ2>
            {
                readonly TResult value;
            }
        }
    }

    [GenerateDiscriminatedUnion]
    public partial class Either<TLeft, TRight>
    {
        partial class Cases
        {
            partial class Left : Either<TLeft, TRight>
            {
                readonly TLeft left;
            }

            partial class Right : Either<TLeft, TRight>
            {
                readonly TRight right;
            }
        }
    }
}
