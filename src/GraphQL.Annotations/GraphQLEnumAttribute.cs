using System;
using GraphQL.Annotations.Types;

namespace GraphQL.Annotations
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class GraphQLEnumAttribute : GraphQLTypeAttribute
    {
        public GraphQLEnumAttribute() : base(typeof(EnumerationGraphType<>))
        {
        }
    }
}
