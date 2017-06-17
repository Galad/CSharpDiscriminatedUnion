using System.Linq;
using NUnit.Framework;
using System;

namespace CSharpDiscriminatedUnion.Generation.Tests
{
    public static class TypeExtensions
    {
        public static string FormatGenericTypeName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }
            var genericTypes = string.Join(", ",
                type.GenericTypeArguments
                    .Select(FormatGenericTypeName));
            return $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
    }
}
