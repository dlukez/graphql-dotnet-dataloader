using System;
using GraphQL;

namespace Serraview.GraphQL.Annotations
{
    public abstract class GraphQLTypeAttribute : Attribute
    {
        protected GraphQLTypeAttribute(Type type)
        {
            if (!type.IsGraphType())
                throw new ArgumentException("Expected a GraphType.", "type");

            GraphType = type;
        }

        public Type GraphType { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
