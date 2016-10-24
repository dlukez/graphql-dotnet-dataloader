using System;
using GraphQL.Annotations.Types;

namespace GraphQL.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public class GraphQLInputObjectAttribute : GraphQLTypeAttribute
    {
        public GraphQLInputObjectAttribute() : base(typeof (InputObjectGraphType<>))
        {
        }
    }
}
