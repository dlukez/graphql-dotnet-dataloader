namespace GraphQL.DataLoader.StarWars.Schema
{
    public class StarWarsSchema : global::GraphQL.Types.Schema
    {
        public StarWarsSchema()
        {
            Query = new StarWarsQuery();
        }
    }
}