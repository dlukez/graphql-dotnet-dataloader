using System;
using Serraview.GraphQL.Annotations.Types;

namespace Serraview.GraphQL.Annotations
{
    [AttributeUsage(AttributeTargets.Enum)]
    public sealed class GraphQLEnumAttribute : GraphQLTypeAttribute
    {
        public GraphQLEnumAttribute() : base(typeof(EnumerationGraphType<>))
        {
        }
    }
}
