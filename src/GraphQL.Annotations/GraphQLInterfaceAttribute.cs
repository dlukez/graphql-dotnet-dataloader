using System;
using GraphQL.Annotations.Types;

namespace GraphQL.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, Inherited = false)]
    public class GraphQLInterfaceAttribute : GraphQLTypeAttribute
    {
        public GraphQLInterfaceAttribute() : base(typeof(InterfaceGraphType<>))
        {
        }
    }
}