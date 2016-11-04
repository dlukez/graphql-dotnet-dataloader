using GraphQL.Types;

namespace GraphQL.Annotations.Types
{
    public class InputObjectGraphType<TModelType> : ComplexGraphType<TModelType>
    {
        public InputObjectGraphType()
        {
            this.ApplyTypeData<TModelType>();
            this.ApplyProperties();
        }

        public override string ToString()
        {
            return Name + " - Input Object Type";
        }
    }
}
