using System;
using System.Linq;
using GraphQL.DataLoader;
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
                .Resolve(new CollectionResolver<Human, Droid>(
                    h => h.HumanId,
                    ids => {
                        Console.WriteLine();
                        Console.WriteLine("=====================");
                        Console.WriteLine("Fetching Human.friends ({0})", string.Join(",", ids));
                        var friends = db.Friendships
                                        .Where(f => ids.Contains(f.HumanId))
                                        .Select(f => new { Key = f.HumanId, f.Droid })
                                        .ToLookup(f => f.Key, f => f.Droid);
                        foreach (var g in friends)
                            Console.WriteLine("  Received Human {0}: Droids ({1})", g.Key, string.Join(",", g));
                        return friends;
                    }));
        }
    }
}
