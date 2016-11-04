using System;
using GraphQL.Types;

namespace GraphQL.Annotations.Types
{
    public class Schema<TRootQuery> : Schema where TRootQuery : class
    {
        public Schema(Func<Type, IGraphType> resolveType) : base(resolveType)
        {
            Query = (IObjectGraphType)resolveType(typeof(TRootQuery));
        }

        public override string ToString()
        {
            return "Schema - " + typeof (TRootQuery).FullName;
        }
    }
}
