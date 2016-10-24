using System.Reflection;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace GraphQL.Extensions.Resolvers
{
    public class PropertyResolver : IFieldResolver
    {
        private readonly PropertyInfo _prop;

        public PropertyResolver(PropertyInfo prop)
        {
            _prop = prop;
        }

        public object Resolve(ResolveFieldContext context)
        {
            return _prop.GetValue(context.Source, null);
        }
    }
}
