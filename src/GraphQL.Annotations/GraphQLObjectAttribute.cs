using System;
using GraphQL.Annotations.Types;

namespace GraphQL.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public class GraphQLObjectAttribute : GraphQLTypeAttribute
    {
        public GraphQLObjectAttribute() : base(typeof (ObjectGraphType<>))
        {
        }
    }
}