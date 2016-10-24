using System;

namespace GraphQL.Annotations
{
    public abstract class GraphQLTypeAttribute : Attribute
    {
        protected GraphQLTypeAttribute(Type type)
        {
            if (!type.IsGraphType())
                throw new ArgumentException("Expected a GraphType.", nameof(type));

            GraphType = type;
        }

        public Type GraphType { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
