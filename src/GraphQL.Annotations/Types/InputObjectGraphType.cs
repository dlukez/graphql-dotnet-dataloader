using GraphQL.Types;

namespace GraphQL.Annotations.Types
{
    public class InputObjectGraphType<TModelType> : ComplexGraphType<TModelType>
    {
        public InputObjectGraphType()
        {
            this.ApplyMetadata<TModelType>();
            this.ImplementFields();
        }

        public override string ToString()
        {
            return Name + " - Input Object Type";
        }
    }
}
