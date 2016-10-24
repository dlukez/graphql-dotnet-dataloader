using System.Linq;
using GraphQL.Extensions.Resolvers;
using GraphQL.TestApp.Data;
using GraphQL.Types;

namespace GraphQL.TestApp.Schema
{
    public class HumanType : ObjectGraphType<Human>
    {
        public HumanType(StarWarsContext db)
        {
            Name = "Human";
            Field(h => h.Name);
            Field(h => h.HumanId);
            Field(h => h.HomePlanet);
            Interface<CharacterInterface>();

            Field<ListGraphType<CharacterInterface>>()
                .Name("friends")
                .Resolve(
                    new CollectionResolver<Human,Droid>(
                        ids => db.Friendships
                                 .Where(f => ids.Contains(f.HumanId))
                                 .Select(f => new { Key = f.HumanId, f.Droid })
                                 .ToLookup(f => f.Key, f => f.Droid),
                        human => human.HumanId)
                    );
        }
    }
}
