using System;
using System.Linq;
using GraphQL.DataLoader;
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
                .Resolve(new CollectionResolver<Droid, Human>(
                    d => d.DroidId,
                    ids => {
                        Console.WriteLine();
                        Console.WriteLine("=====================");
                        Console.WriteLine("Fetching Droid.friends ({0})", string.Join(",", ids));
                        var friends = db.Friendships
                                        .Where(f => ids.Contains(f.DroidId))
                                        .Select(f => new { Key = f.DroidId, f.Human })
                                        .ToLookup(x => x.Key, x => x.Human);
                        Console.WriteLine("Got Droid.friends:");
                        foreach (var g in friends)
                            Console.WriteLine("  Received Droid {0}: Humans ({1})", g.Key, string.Join(",", g));
                        return friends;
                    }));
        }
    }
}
