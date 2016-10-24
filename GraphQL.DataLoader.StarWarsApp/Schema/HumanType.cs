using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.DataLoader.StarWarsApp.Data;
using GraphQL.Types;

namespace GraphQL.DataLoader.StarWarsApp.Schema
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

            Func<IEnumerable<int>, ILookup<int, Droid>> fetchFriends = ids =>
            {
                Console.WriteLine("Fetching friends of humans " + string.Join(", ", ids));
                using (var db = new StarWarsContext())
                    return db.Friendships
                        .Where(f => ids.Contains(f.HumanId))
                        .Select(f => new {Key = f.HumanId, f.Droid})
                        .ToLookup(f => f.Key, f => f.Droid);
            };

            // Using the loader
            var friendsLoader = new DataLoader<int, Droid>(fetchFriends);
            Field<ListGraphType<CharacterInterface>>()
                .Name("friends")
                .Resolve(ctx => friendsLoader.LoadAsync(ctx.Source.HumanId));

            // Using the resolver
            var friendsResolver = new DataLoaderResolver<Human, Droid>(h => h.HumanId, fetchFriends);
            Field<ListGraphType<CharacterInterface>>()
                .Name("friends2")
                .Resolve(friendsResolver);
        }
    }
}
