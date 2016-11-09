using System;
using Serraview.GraphQL.Annotations.Types;

namespace Serraview.GraphQL.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public class GraphQLObjectAttribute : GraphQLTypeAttribute
    {
        public GraphQLObjectAttribute() : base(typeof (ObjectGraphType<>))
        {
        }
    }
}