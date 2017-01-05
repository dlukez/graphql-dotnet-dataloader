/*
 *  Example 2: Each request has its own context passed through the UserContext.
 *
 *    This helps keep queries/batches separate and is more likely to prevent
 *    issues arising from other queries that are executing concurrently.
 */

using System.Linq;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.DataLoader.StarWars.Schema
{
    public class HumanType : ObjectGraphType<Human>
    {
        public HumanType()
        {
            Name = "Human";
            Field(h => h.Name);
            Field(h => h.HumanId);
            Field(h => h.HomePlanet);
            Interface<CharacterInterface>();

            Field<ListGraphType<CharacterInterface>>()
                .Name("friends")
                .Resolve(ctx => ctx.GetDataLoader(async ids =>
                    {
                        var db = ctx.GetDataContext();
                        return (await db.Friendships
                            .Where(f => ids.Contains(f.HumanId))
                            .Select(f => new {Key = f.HumanId, f.Droid})
                            .ToListAsync())
                            .ToLookup(x => x.Key, x => x.Droid);
                    }).LoadAsync(ctx.Source.HumanId));

            Field<ListGraphType<EpisodeType>>()
                .Name("appearsIn")
                .Resolve(ctx => ctx.GetDataLoader(async ids =>
                    {
                        var db = ctx.GetDataContext();
                        return (await db.HumanAppearances
                            .Where(ha => ids.Contains(ha.HumanId))
                            .Select(ha => new { Key = ha.HumanId, ha.Episode })
                            .ToListAsync())
                            .ToLookup(x => x.Key, x => x.Episode);
                    }).LoadAsync(ctx.Source.HumanId));
        }
    }
}
