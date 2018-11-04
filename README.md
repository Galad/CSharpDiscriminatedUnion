
# CSharpDiscriminatedUnion

[![Build status](https://ci.appveyor.com/api/projects/status/kkfso0qs6hfer05h?svg=true)](https://ci.appveyor.com/project/Galad/csharpdiscriminatedunion)

CSharpDiscriminatedUnion is code generation library that allow C# developers to create and use [discriminated unions](https://docs.microsoft.com/en-us/dotnet/fsharp/language-reference/discriminated-unions) in C#.

It relies on [CodeGeneration.Roslyn](https://github.com/AArnott/CodeGeneration.Roslyn) to generate C# code.

## Table of Contents

* [Overview][]
* [Installation][]
* [Template reference (class)][]
* [Template reference (struct)][]
* [Case values][]
* [Using a struct][]
* [Equality][]
* [Match method][]

## Overview
[Overview]: #overview
CSharpDiscriminatedUnion allows you to describe the base implementation of a discriminated union, and generates all the factory methods and the case matching method for you, as well as the equality implementations.

Here is an example of a [Maybe](https://wiki.haskell.org/Maybe) type created with CSharpDiscriminatedUnion

```csharp
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
```

The attribute enables the code generation for the type it annotates. 
The type must be partial.
It can be a class or a structure. There is a slight difference on how class and structure discriminated union types are declared (see below for more details);

A case is identified by a nested class that inherits the union type (here, `Maybe<T>`). The class name is the case identifier, and is used for the factory methods.
Cases must always be created in the nested static class Cases (`static partial class Cases`), which is generated automatically.

Once the code is generated, you have access to factory methods to create new instances, and a `Match` method, similar to pattern matching, that allows to deconstruct the value.

Here are some examples of what you can do with the type `Maybe<T>`

```csharp
    // create a case None value
    var none = Maybe<int>.None;
    var three = Maybe<int>.Just(3);
    var otherThree = Maybe<int>.Just(3);
    var four = Maybe<int>.Just(3);
    
    Console.WriteLine(none); //writes "None"
    Console.WriteLine(someValue); //writes "3"
    Console.WriteLine(three == otherThree); //writes "true"
    Console.WriteLine(three == none); //writes "false"
    Console.WriteLine(three == four); //writes "false"
    
    private string MaybeToString(Maybe<int> value)
    {
       return value.Match(() => "None", i => i.ToString());
    }
```

## Installation
[Installation]: #installation
* Install the NuGet package CSharpDiscriminatedUnion
```xml
    <PackageReference Include="CSharpDiscriminatedUnion" Version="1.0.0" PrivateAssets="all" />
```
* Add the required dotnet CLI tool:
```xml
    <DotNetCliToolReference Include="dotnet-codegen" Version="0.4.88" />
```
Make sure that the version of the tool matches the version of CodeGeneration that is installed.

For more information see https://github.com/AArnott/CodeGeneration.Roslyn#packaging-up-your-code-generator-for-others-use

## Template reference (class)
[Template reference (class)]: #template-reference-class

```csharp
    [GenerateDiscriminatedUnion(CaseFactoryPrefix = "New", PreventNullValues = false)]
    partial class [<name>]
    {
        static partial class Cases
        {
            // 0 or more classes
            /// <summary>
            /// [<case description>]
            /// </summary>            
            partial class [<case1Name>] : [<name>]
            {
                // 0 or more fields
                /// <summary>
                /// [<value description>]
                /// </summary>                
                readonly [type] [<case1Value>];                
            }
        }
    }
```

## Template reference (struct)
[Template reference (struct)]: #template-reference-struct

```csharp
    [GenerateDiscriminatedUnion(CaseFactoryPrefix = [<factory method prefix>], PreventNullValues = (true|false))]
    partial class [<name>]
    {
        // 0 or more fields per case
        /// <summary>
        /// [<value description>]
        /// </summary>       
        [StructCase([<case1name>], isDefaultValue: (true|false), description: [<case description>])]        
        readonly [type] [<case1Value>];

        // Parameterless cases. 0 or more attribute declaration, one for each case
        [StructCase([<case2name>], description: [<description>])]        
        static partial class Cases
        {          
        }
    }
```

## Case values
[Case values]: #case-values
A case may contain 0 or more fields that represent its value.
When a case does not contain any, there is just one single possible value for this case. This single value is represented as a static field in the class.

```csharp
    [GenerateDiscriminatedUnion]
    public class Unit
    {
        static partial class Cases
        {
            partial class Default : Unit { }
        }
    }
    
    var unit = Unit.Default;
```

When a case has one or more values, a factory method is generated. It takes a value as an argument for each readonly field in the case class.

```csharp
    [GenerateDiscriminatedUnion]
    public class Mammal
    {
        static partial class Cases
        {
            partial class Human : Mammal
            {
                readonly string firstName;
                readonly string lastName;
            }
            
            partial class Dog : Mammal
            {
                readonly string name;
            }
        }
    }
    
    var human = Mammal.NewHuman("Alan", "Turing");
    var dog = Mammal.NewDog("Bob");
```

## Using a struct
[Using a struct]: #using-a-struct
You can also declare a discriminated union type as a struct.
The declaration is slightly different, because its internal representation is different. Each parameter must be declared as a private field of the type, and must be linked to the case with the `StructCase` attribute.
You must also use the `StructCase` attribute to declare cases that do not use parameters.
In addition, if you can specifiy which case is the default value of the struct (`var mammal = default(Mammal);`).

Here is an example: 

```csharp
    [GenerateDiscriminatedUnion]
    public struct Mammal
    {
        [StructCase("Human")]
        readonly string firstName;
        [StructCase("Human")]
        readonly string lastName;
        [StructCase("Dog", isDefaultValue: true)]
        readonly string name;

        [StructCase("Cow")]
        static partial class Cases
        {
        }
    }
    
    var human = Mammal.NewHuman("Alan", "Turing");
    var dog = Mammal.NewDog("Bob");
    var cow = Mammal.Cow;
    var mammal = default(Mammal); // it is a dog. Note that the name property will be null;    
```

## Equality
[Equality]: #equality
Union types represent a value, therefore if a case have values, all those values are considered in the equality comparison.
2 instances that represent a different case cannot be equal.
If 2 instances represent the same case, their values are compared using the Equals method.
Here are some examples:

```csharp 
    Mammal.NewHuman("Alan", "Turing") == Mammal.NewHuman("Alan", "Turing"); // returns true
    Mammal.NewHuman("Alan", "Turing") != Mammal.NewHuman("Alan", "Turing"); // returns false
    Mammal.NewHuman("Alan", "Turing") == Mammal.NewHuman("Alan", "Stivell"); // returns false    
    Mammal.NewHuman("Alan", "Turing") == Mammal.NewDog("Bob"); // returns false
```

## Match method
[Match method]: #match-method
A method named `Match` is generated.
When using it, you should provide 1 function for each possible case of the type. This allows to make sure that every possible case can be handled by the code using it.
Here are some examples:

```csharp
    string IsHuman(Mammal mammal) => 
    {
        Match(
          (firstName, lastName) => true,
          name => false,
          name => false
          ));    
    }
    IsHuman(Mammal.NewHuman("Alan", "Turing")); //return true
    IsHuman(Mammal.NewDog("Bob")); //return false
    IsHuman(Mammal.NewCat("Bob")); //return false    
```

Alternatively, you can use an overload that does not requires to provide a function for all the cases.

```csharp
    string IsHuman(Mammal mammal) => 
    {
        Match(
          () => false,
          matchHuman: (firstName, lastName) => true
          ));    
    }
    IsHuman(Mammal.NewHuman("Alan", "Turing")); //return true
    IsHuman(Mammal.NewDog("Bob")); //return false
    IsHuman(Mammal.NewCat("Bob")); //return false    
```