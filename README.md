DataLoader for .NET
===================

A port of Facebook's [DataLoader](https://github.com/facebook/dataloader) for .NET.

Originally began as a solution to the [select N+1 problem](https://github.com/graphql-dotnet/graphql-dotnet/issues/21)
for [GraphQL in .NET](https://github.com/graphql-dotnet/graphql-dotnet) but found that most of the (small amount of) code
was independent and could be generalized for use in other scenarios.

If anyone finds use for this in other areas, please let me know...
I'd love to know whether the solution could be expanded to cater for other uses.


Caveats
-------

Facebook's implementation runs in Javascript and takes advantage of the
[event loop](https://developer.mozilla.org/en-US/docs/Web/API/window/requestAnimationFrame)
to fire any pending requests for ID's collected during the previous frame.
Unfortunately not all .NET applications run in an event loop.

For this reason, we have our own `DataLoaderContext` to house `DataLoader` instances.
Any instances should be called within a particular `DataLoaderContext` - which essentially
represents a frame in Javascript - using the static `DataLoaderContext.Run` method.
This method will run the user-supplied delegate before calling `Start` on the created context,
which then fires any pending fetches and processes the results.

Loaders may be called again as the results are processed, which would cause them to be requeued.
This effectively turns the context into a kind of asynchronous loader pump.


Usage
-----

```csharp
public void GetPersonsManually()
{
    var personLoader = new DataLoader<int, Person>(ids =>
    {
        using (var db = new StarWarsContext())
            return db.Person.Where(p => ids.Contains(p.Id)).ToListAsync();
    });

    var person1 = personLoader.LoadAsync(1);
    var person2 = personLoader.LoadAsync(2);
    var person3 = personLoader.LoadAsync(3);
    var task = Task.WhenAll(person1, person2, person3);

    // Do some stuff when they're all loaded
    task.ContinueWith(_ => Console.WriteLine("Hello there " + string.Join(', ', _.Result.Select(p => p.Name))));

    // Actually trigger the load
    personLoader.ExecuteAsync();

    return task;
}

public void GetPersonsContextual()
{
    // The collect/fire cycle can be managed implicitly using
    // the static DataLoaderContext.Run() method.
    var result = await DataLoaderContext.Run(() =>
    {
        // Implicit context here... DataLoaderContext.Current != null
        // and is used by the loader during a call to LoadAsync.
        var task1 = loader.LoadAsync(1);
        var task2 = loader.LoadAsync(2);
        var task3 = loader.LoadAsync(3);
        return await Task.WhenAll(task1, task2, task3).ConfigureAwait(false);
    });

    Console.WriteLine(result[0]);
    Console.WriteLine(result[1]);
    Console.WriteLine(result[2]);
}
```

### GraphQL example

```csharp
// Example 1 - Create and use loaders directly.
//   This approach does not require the GraphQL.DataLoader package
//   and can instead depend on only the main DataLoader package.
var friendsLoader = new DataLoader<int, Droid>(ids => {
    using (var db = new StarWarsContext())
        return db.Friendships
            .Where(f => ids.Contains(f.HumanId))
            .Select(f => new {Key = f.HumanId, f.Droid})
            .ToLookup(f => f.Key, f => f.Droid);
});

Field<ListGraphType<CharacterInterface>>()
    .Name("friends2")
    .Resolve(ctx => friendsLoader.LoadAsync(ctx.Source.HumanId));

// Example 2 - resolve field context extension method.
//   Creates or reuses a loader for the given field, keyed by the
//   field definition object. This approach could be implemented
//   using only the DataLoader package
Field<ListGraphType<CharacterInterface>>()
    .Name("friends3")
    .Resolve(ctx => ctx.GetDataLoader(ids => {
        using (var db = new StarWarsContext())
            return db.Friendships
                .Where(f => ids.Contains(f.HumanId))
                .Select(f => new {Key = f.HumanId, f.Droid})
                .ToLookup(f => f.Key, f => f.Droid);
    }));

// Example 3 - field builder BatchResolve extension method
//   Creates a 
Field<ListGraphType<CharacterInterface>>()
    .Name("friends4")
    .BatchResolve((ids/*, ctx*/) => {
        using (var db = new StarWarsContext())
            return db.Friendships
                .Where(f => ids.Contains(f.HumanId))
                .Select(f => new {Key = f.HumanId, f.Droid})
                .ToLookup(f => f.Key, f => f.Droid);
    });
```


Running the sample app
----------------------

```
cd example/
dotnet ef migrations add InitialSetup
dotnet ef database update
dotnet run
```


To do
-----
[x] Basic support
[x] Support async fetches
[ ] Cancellation
[ ] Benchmarks
[ ] Multithreaded performance