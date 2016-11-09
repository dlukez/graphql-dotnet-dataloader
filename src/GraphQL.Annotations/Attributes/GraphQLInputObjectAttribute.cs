using System;
using Serraview.GraphQL.Annotations.Types;

namespace Serraview.GraphQL.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
    public class GraphQLInputObjectAttribute : GraphQLTypeAttribute
    {
        public GraphQLInputObjectAttribute() : base(typeof (InputObjectGraphType<>))
        {
        }
    }
}
