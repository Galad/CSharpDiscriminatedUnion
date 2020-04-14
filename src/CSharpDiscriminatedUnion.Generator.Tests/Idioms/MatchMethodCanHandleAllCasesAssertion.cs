using CSharpDiscriminatedUnion.Attributes;
using Moq;
using Moq.Language.Flow;
using NUnit.Framework;
using AutoFixture.Idioms;
using AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDiscriminatedUnion.Generator.Tests.Idioms
{
    /// <summary>
    /// Verify that a Match method can handle all the declared cases, ie there is a Func parameter for each case
    /// </summary>
    public sealed class MatchMethodCanHandleAllCasesAssertion : IdiomaticAssertion
    {
        private readonly ISpecimenBuilder specimenBuilder;

        public MatchMethodCanHandleAllCasesAssertion(ISpecimenBuilder specimenBuilder)
        {
            this.specimenBuilder = specimenBuilder ?? throw new ArgumentNullException(nameof(specimenBuilder));
        }

        public override void Verify(Type type)
        {
            var testedType = type;
            if (type.IsGenericTypeDefinition)
            {
                testedType = type.MakeGenericType(type.GetGenericArguments().Select(_ => typeof(int)).ToArray());
            }
            var matchMethods = testedType.GetMethods().Where(m => m.Name == "Match" && m.GetParameters().All(p => p.Name != "none"));
            if (!matchMethods.Any())
            {
                ThrowAssertionException(testedType, $"The type {testedType.Name} does not contain a Match method.");
                return;
            }

            var cases = GetCases(testedType).ToArray();
            foreach (var matchMethod in matchMethods)
            {
                VerifyMatchMethod(testedType, matchMethod, cases);
            }
            base.Verify(testedType);
        }

        private static void ThrowAssertionException(Type type, string message)
        {
            var classOrStruct = type.IsClass ? "class" : "struct";
            throw new AssertionException(
                message +
                $"\nThe type is a {classOrStruct}");
        }

        private void VerifyMatchMethod(Type type, MethodInfo matchMethod, UnionCase[] cases)
        {
            object create(Type t)
            {
                return specimenBuilder.Create(t, new SpecimenContext(specimenBuilder));
            }
            var expected = create(typeof(object));

            foreach (var @case in cases)
            {
                object sut;
                object[] values;
                if (@case.Values.Length == 0)
                {
                    values = new object[] { };
                    sut = type.GetField(@case.Name, BindingFlags.Public | BindingFlags.Static)
                              .GetValue(null);
                }
                else
                {
                    values = @case.Values.Select(v => create(v.FieldType)).ToArray();
                    sut = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                              .Single(m => IsCase(m.Name, @case.Name))
                              .Invoke(null, values);
                }
                object createFuncMock(Type funcType, bool expectedCase)
                {
                    var mock = typeof(Mock<>).MakeGenericType(funcType)
                                             .GetConstructor(new Type[] { })
                                             .Invoke(new object[] { });
                    // create expected values
                    var genericArguments = funcType.GetGenericArguments();
                    var inputTypes = genericArguments.Take(genericArguments.Length - 1);
                    var inputValues = inputTypes.Select((t, j) =>
                    {
                        if (expectedCase)
                        {
                            return Expression.Constant(values[j]);
                        }
                        var isAnyMethod = typeof(It).GetMethod(nameof(It.IsAny), BindingFlags.Static | BindingFlags.Public)
                                                    .MakeGenericMethod(t);
                        return (Expression)Expression.Call(isAnyMethod);
                    });
                    // create setup expression
                    var parameter = Expression.Parameter(funcType, "func");
                    var body = Expression.Invoke(
                        parameter,
                        inputValues.ToArray());
                    var lambda = Expression.Lambda(body, parameter);
                    var setup = mock.GetType().GetMethods().Where(m => m.Name == "Setup").Skip(1).Single()
                                              .MakeGenericMethod(typeof(object))
                                              .Invoke(mock, new object[] { lambda });
                    setup.GetType()
                         .GetMethod("Returns", new[] { typeof(object) })
                         .Invoke(setup, new object[] { expectedCase ? expected : null });

                    return (mock as Mock).Object;
                }

                var parameters = matchMethod
                                          .MakeGenericMethod(typeof(object))
                                          .GetParameters()
                                          .Select(p => (p, c: cases.First(c => c.Name == p.Name.Remove(0, "match".Length))))
                                          .Select((p, i) => createFuncMock(p.p.ParameterType, p.c.Name == @case.Name))
                                          .ToArray();
                var actual = matchMethod.MakeGenericMethod(typeof(object)).Invoke(sut, parameters);
                if (actual != expected)
                {
                    ThrowAssertionException(type, $"Method {matchMethod}, with case {@case.Name}, did not match the value as expected.");
                }
            }
        }

        private bool IsCase(string methodName, string caseName)
        {
            return methodName.StartsWith("New") &&
                   methodName.Remove(0, "New".Length) == caseName ||
                   methodName.StartsWith("Create") &&
                   methodName.Remove(0, "Create".Length) == caseName ||
                   methodName == caseName;
        }

        private IEnumerable<UnionCase> GetCases(Type type)
        {

            var caseClass = type.GetNestedType("Cases", BindingFlags.NonPublic | BindingFlags.Static);
            if (caseClass == null)
            {
                ThrowAssertionException(type, $"The Cases class is missing for type {type.Name}");
            }
            // class
            if (type.IsClass)
            {
                var cases = caseClass.GetNestedTypes()
                                     .Select(c =>
                                     {
                                         if (c.IsGenericTypeDefinition)
                                         {
                                             c = c.MakeGenericType(type.GenericTypeArguments);
                                         }
                                         return new UnionCase(c.Name, c.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
                                     });
                return cases;
            }
            // struct
            var valuelessCases = caseClass.GetCustomAttributesData()
                                          .Where(d => d.AttributeType == (typeof(StructCaseAttribute)))
                                          .Select(a =>
                                                new UnionCase(
                                                    a.ConstructorArguments[0].Value.ToString(),
                                                    new FieldInfo[] { }
                                                    )
                                                );
            var fieldInfoCases = type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                                     .Where(f => f.Name != "_tag")
                                     .Select(f => (f.GetCustomAttributesData()
                                                   .Single(d => d.AttributeType == (typeof(StructCaseAttribute)))
                                                   .ConstructorArguments[0].Value.ToString(),
                                                   f)
                                            )
                                     .GroupBy(f => f.Item1)
                                     .Select(g => new UnionCase(g.Key, g.Select(gg => gg.f).ToArray()));
            return valuelessCases.Concat(fieldInfoCases);
        }

        private class UnionCase
        {
            public string Name { get; }
            public string FieldName
            {
                get
                {
                    var chars = Name.ToCharArray();
                    chars[0] = char.ToLower(chars[0]);
                    return new string(chars);
                }
            }

            public FieldInfo[] Values { get; }

            public UnionCase(string Name, FieldInfo[] Values)
            {
                this.Name = Name;
                this.Values = Values;
            }
        }
    }
}
