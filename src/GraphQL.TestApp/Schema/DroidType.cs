using System.Linq;
using GraphQL.Extensions.Resolvers;
using GraphQL.TestApp.Data;
using GraphQL.Types;

namespace GraphQL.TestApp.Schema
{
    public class DroidType : ObjectGraphType<Droid>
    {
        public DroidType(StarWarsContext db)
        {
            Name = "Droid";
            Field(d => d.Name);
            Field(d => d.DroidId);
            Field(d => d.PrimaryFunction);
            Interface<CharacterInterface>();

            Field<ListGraphType<CharacterInterface>>()
                .Name("friends")
                .Resolve(
                    new CollectionResolver<Droid, Human>(
                        ids => db.Friendships
                                 .Where(f => ids.Contains(f.DroidId))
                                 .Select(f => new {Key = f.DroidId, f.Human})
                                 .ToLookup(x => x.Key, x => x.Human),
                        droid => droid.DroidId)
                    );
        }
    }
}
