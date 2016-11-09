using System;
using Serraview.GraphQL.Annotations.Types;

namespace Serraview.GraphQL.Annotations
{
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class, Inherited = false)]
    public class GraphQLScalarAttribute : GraphQLTypeAttribute
    {
        public GraphQLScalarAttribute() : base(typeof(ScalarGraphType<>))
        {
        }
    }
}
