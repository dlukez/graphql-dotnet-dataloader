using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.DataLoader.StarWars.Schema
{
    public class StarWarsQuery : ObjectGraphType
    {
        public StarWarsQuery()
        {
            Name = "Query";

            Field<ListGraphType<HumanType>>()
                .Name("humans")
                .Resolve(ctx => ctx.GetDataContext().Humans.ToListAsync());

            Field<ListGraphType<DroidType>>()
                .Name("droids")
                .Resolve(ctx => ctx.GetDataContext().Droids.ToListAsync());

            Field<ListGraphType<EpisodeType>>()
                .Name("episodes")
                .Resolve(ctx => ctx.GetDataContext().Episodes.ToListAsync());
        }
    }
}
