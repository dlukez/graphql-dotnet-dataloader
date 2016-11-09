using System;
using GraphQL.Types;

namespace Serraview.GraphQL.Annotations.Types
{
    public class Schema<TRootQuery> : Schema where TRootQuery : class
    {
        public Schema(params object[] injectedObjects)
        {
            ResolveType = t =>
            {
                var genericType = t.IsGenericType ? t.GetGenericTypeDefinition() : null;
                if (genericType != null &&
                    (genericType == typeof(ObjectGraphType<>) || genericType == typeof(InterfaceGraphType<>)))
                    return (GraphType) Activator.CreateInstance(t, injectedObjects);
                return (GraphType) Activator.CreateInstance(t);
            };

            Query = (IObjectGraphType)ResolveType(typeof(ObjectGraphType<TRootQuery>));
        }

        public override string ToString()
        {
            return "Schema - " + typeof (TRootQuery).FullName;
        }
    }
}
