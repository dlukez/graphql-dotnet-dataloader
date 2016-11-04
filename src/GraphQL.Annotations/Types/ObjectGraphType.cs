using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GraphQL.Annotations.Types
{
    public class ObjectGraphType<TModelType> : GraphQL.Types.ObjectGraphType<TModelType>
        where TModelType : class
    {
        public ObjectGraphType()
        {
            this.ApplyTypeData<TModelType>();
            this.ApplyProperties();
            this.ApplyMethods(true);
            ImplementInterfaces();
        }

        private void ImplementInterfaces()
        {
            var type = typeof (TModelType);
            var baseTypes = GetBaseTypes(type).Where(t => t.GetTypeInfo().IsAbstract);
            var interfaces = type.GetInterfaces();
            var implementedInterfaces = baseTypes.Concat(interfaces);
            var genericInterfaceType = typeof(InterfaceGraphType<>);
            foreach (var implementedInterface in implementedInterfaces)
            {
                var interfaceAttr = implementedInterface.GetTypeInfo().GetCustomAttribute<GraphQLInterfaceAttribute>(false);
                if (interfaceAttr == null)
                    continue;

                // Call Interface(typeof(InterfaceGraphType<Interface>));
                Interface(genericInterfaceType.MakeGenericType(implementedInterface));
            }
        }

        private static IEnumerable<Type> GetBaseTypes(Type type)
        {
            while (type.GetTypeInfo().BaseType != null)
            {
                type = type.GetTypeInfo().BaseType;
                yield return type;
            }
        }

        public override string ToString()
        {
            return Name + " - Object Type";
        }
    }
}
