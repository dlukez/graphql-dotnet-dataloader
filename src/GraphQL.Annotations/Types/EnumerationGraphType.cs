using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using GraphQL.Types;

namespace Serraview.GraphQL.Annotations.Types
{
    /// <summary>
    /// Copy of EnumType[T] from GraphQL.Tests.
    /// See https://github.com/graphql-dotnet/graphql-dotnet/blob/master/src/GraphQL.Tests/Bugs/Bug68NonNullEnumGraphTypeTests.cs#L89.
    /// </summary>
    public class EnumerationGraphType<T> : EnumerationGraphType
      where T : struct
    {
        public EnumerationGraphType()
        {
            if (!typeof(T).GetTypeInfo().IsEnum)
            {
                throw new ArgumentException(typeof(T).Name + " must be of type enum");
            }

            var type = typeof(T);
            Name = DeriveGraphQlName(type.Name);

            foreach (var enumName in type.GetTypeInfo().GetEnumNames())
            {
                var enumMember = type
                  .GetMember(enumName, BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)
                  .First();

                var name = DeriveEnumValueName(enumMember.Name);

                AddValue(name, null, Enum.Parse(type, enumName));
            }
        }

        public override object ParseValue(object value)
        {
            var found = Values.FirstOrDefault(
              v =>
                StringComparer.OrdinalIgnoreCase.Equals(PureValue(v.Name), PureValue(value)) ||
                StringComparer.OrdinalIgnoreCase.Equals(PureValue(v.Value.ToString()), PureValue(value)));
            return found != null ? found.Value : null;
        }

        public object GetValue(object value)
        {
            var found =
              Values.FirstOrDefault(
                v => StringComparer.OrdinalIgnoreCase.Equals(PureValue(v.Name), PureValue(value)));
            return found != null ? found.Value : null;
        }

        static string PureValue(object value)
        {
            return value.ToString().Replace("\"", "").Replace("'", "").Replace("_", "");
        }

        static string DeriveGraphQlName(string name)
        {
            return char.ToUpperInvariant(name[0]) + name.Substring(1);
        }

        static string DeriveEnumValueName(string name)
        {
            return Regex
              .Replace(name, @"([A-Z])([A-Z][a-z])|([a-z0-9])([A-Z])", "$1$3_$2$4")
              .ToUpperInvariant();
        }
    }
}
