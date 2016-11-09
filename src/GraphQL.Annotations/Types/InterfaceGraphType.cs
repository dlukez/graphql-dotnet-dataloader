using System;
using System.Reflection;

namespace Serraview.GraphQL.Annotations.Types
{
    public class InterfaceGraphType<TModelType> : global::GraphQL.Types.InterfaceGraphType<TModelType>
    {
        public InterfaceGraphType(params object[] injectedParameters)
        {
            var type = typeof (TModelType);
            this.ApplyTypeData<TModelType>();
            this.ApplyProperties();
            this.ApplyMethods(injectedParameters, false);
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
