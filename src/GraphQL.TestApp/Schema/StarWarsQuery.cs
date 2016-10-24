using System.Linq;
using GraphQL.TestApp.Data;
using GraphQL.Types;

namespace GraphQL.TestApp.Schema
{
    public class StarWarsQuery : ObjectGraphType
    {
        public StarWarsQuery(StarWarsContext db)
        {
            Name = "Query";

            Field<ListGraphType<HumanType>>(
                "humans",
                resolve: context => db.Humans.ToList());

            Field<ListGraphType<DroidType>>(
                "droids",
                resolve: context => db.Droids.ToList());
        }
    }
}
