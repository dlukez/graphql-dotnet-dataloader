using System;
using Serraview.GraphQL.Annotations.Types;

namespace Serraview.GraphQL.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
    public class GraphQLInterfaceAttribute : GraphQLTypeAttribute
    {
        public GraphQLInterfaceAttribute() : base(typeof(InterfaceGraphType<>))
        {
        }
    }
}