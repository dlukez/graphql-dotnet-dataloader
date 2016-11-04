using System.ComponentModel;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace GraphQL.Annotations.Types
{
    public class ScalarGraphType<TModelType> : ScalarGraphType
    {
        public ScalarGraphType()
        {
            this.ApplyTypeData<TModelType>();
        }

        public override string ToString()
        {
            return Name + " - Scalar Type";
        }

        public override object Serialize(object value)
        {
            return ParseValue(value);
        }

        public override object ParseValue(object value)
        {
            if (value is TModelType)
                return value;

            var converter = TypeDescriptor.GetConverter(typeof(TModelType));
            if (converter.CanConvertFrom(value.GetType()))
                return converter.ConvertFrom(value);

            return null;
        }

        public override object ParseLiteral(IValue value)
        {
            var stringValue = value as StringValue;
            if (stringValue != null)
                return ParseValue(stringValue.Value.Trim(' ', '"'));

            return null;
        }
    }
}
