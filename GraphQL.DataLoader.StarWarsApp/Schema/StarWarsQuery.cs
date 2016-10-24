using System.Linq;
using GraphQL.DataLoader.StarWarsApp.Data;
using GraphQL.Types;

namespace GraphQL.DataLoader.StarWarsApp.Schema
{
    public class StarWarsQuery : ObjectGraphType
    {
        public StarWarsQuery()
        {
            Name = "Query";

            Field<ListGraphType<HumanType>>()
                .Name("humans")
                .Resolve(ctx =>
                {
                    using (var db = new StarWarsContext())
                        return db.Humans.ToList();
                });

            Field<ListGraphType<DroidType>>()
                .Name("droids")
                .Resolve(ctx =>
                {
                    using (var db = new StarWarsContext())
                        return db.Droids.ToList();
                });
        }
    }
}
