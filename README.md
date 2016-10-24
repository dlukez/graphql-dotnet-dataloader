GraphQL.DataLoader
==================

One solution to the [select N+1 problem](https://github.com/graphql-dotnet/graphql-dotnet/issues/21) in .NET.

Contents
--------

Small server using [GraphQL](http://github.com/graphql-dotnet/graphql-dotnet) and an implementation of [DataLoader](http://github.com/facebook/dataloader) in .NET.

+ __GraphQL.DataLoader__ - Contains the DataLoader classes and a GraphQL resolver.
+ __GraphQL.DataLoader.StarWarsApp__ - Example usage.

If people find this useful I may publish it as a NuGet package - for now it's just for reference.

Setup the sample app
--------------------
```
cd src/GraphQL.DataLoader.StarWarsApp/
dotnet ef migrations add InitialSetup
dotnet ef database update
dotnet run
```

API
---

```csharp
// Example 1 - FieldBuilder resolve overload extension method
Field<ListGraphType<CharacterInterface>>()
    .Name("friends1")
    .Resolve(h => h.HumanId, ids =>
    {
        using (var db = new StarWarsContext())
            return db.Friendships
                .Where(f => ids.Contains(f.HumanId))
                .Select(f => new {Key = f.HumanId, f.Droid})
                .ToLookup(f => f.Key, f => f.Droid);
    });

// Example 2 - ResolveFieldContext extension method
Field<ListGraphType<CharacterInterface>>()
    .Name("friends4")
    .Resolve(ctx => ctx.GetBatchLoader(ids =>
    {
        using (var db = new StarWarsContext())
            return db.Friendships
                .Where(f => ids.Contains(f.HumanId))
                .Select(f => new {Key = f.HumanId, f.Droid})
                .ToLookup(f => f.Key, f => f.Droid);
    }).LoadAsync(ctx.Source.HumanId));

// Example 3 - manually wire up a loader
var friendsLoader = new DataLoader<Droid>(ids =>
    {
        using (var db = new StarWarsContext())
            return db.Friendships
                .Where(f => ids.Contains(f.HumanId))
                .Select(f => new {Key = f.HumanId, f.Droid})
                .ToLookup(f => f.Key, f => f.Droid);
    });

Field<ListGraphType<CharacterInterface>>()
    .Name("friends2")
    .Resolve(ctx => friendsLoader.LoadAsync(ctx.Source.HumanId));

// Example 4 - manually specify a resolver
var friendsResolver = new DataLoaderResolver<Human, Droid>(h => h.HumanId, ids =>
    {
        using (var db = new StarWarsContext())
            return db.Friendships
                .Where(f => ids.Contains(f.HumanId))
                .Select(f => new {Key = f.HumanId, f.Droid})
                .ToLookup(f => f.Key, f => f.Droid);
    });

Field<ListGraphType<CharacterInterface>>()
    .Name("friends3")
    .Resolve(friendsResolver);
```
