namespace GraphQL.DataLoader.StarWarsApp.Schema
{
    public class StarWarsSchema : GraphQL.Types.Schema
    {
        public StarWarsSchema()
        {
            Query = new StarWarsQuery();
        }
    }
}