using System;
using System.Reflection;

namespace GraphQL.Annotations.Types
{
    public class InterfaceGraphType<TModelType> : GraphQL.Types.InterfaceGraphType<TModelType>
    {
        public InterfaceGraphType()
        {
            var type = typeof (TModelType);
            this.ApplyMetadata<TModelType>();
            this.ImplementFields();
            Name = GetInterfaceName(type);
        }

        private static string GetInterfaceName(Type type)
        {
            return type.GetTypeInfo().IsInterface ? type.Name.Substring(1) : type.Name;
        }

        public override string ToString()
        {
            return Name + " - Interface Type";
        }
    }
}
