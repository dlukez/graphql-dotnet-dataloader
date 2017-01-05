using DataLoader;
using System.Linq;
using System.Threading.Tasks;
using GraphQL.Types;
using Shouldly;
using Xunit;
using System.Collections.Generic;
using System;

namespace DataLoader.GraphQL.Tests
{
    public class DataLoaderResolverTests
    {
        [Fact]
        public async void DataLoaderResolver_Resolve_WithFieldContext()
        {
            ResolveFieldContext ctx = null;
            ResolveFieldContext lastCtx = null;

            var resolver = new DataLoaderResolver<int, int, int>(
                x => x,
                (IEnumerable<int> ids, ResolveFieldContext fieldCtx) =>
                {
                    Console.WriteLine("Resolving ");
                    lastCtx = fieldCtx;
                    return Task.FromResult(ids.ToLookup(id => id, id => id + 1));
                });

            Console.WriteLine("Task 1");
            ctx = new ResolveFieldContext() { Source = 0 };
            var task1 = resolver.Resolve(ctx);

            Console.WriteLine("Task 2");
            ctx = new ResolveFieldContext() { Source = 1 };
            var task2 = resolver.Resolve(ctx);

            Console.WriteLine("Task 3");
            ctx = new ResolveFieldContext() { Source = 2 };
            var task3 = resolver.Resolve(ctx);

            Console.WriteLine("Awaiting...");

            await resolver.ExecuteAsync();
            var result = await Task.WhenAll(new [] { task1, task2, task3 });
            lastCtx.ShouldBeSameAs(ctx);
            result[0].Single().ShouldBe(1);
            result[1].Single().ShouldBe(2);
            result[2].Single().ShouldBe(3);
        }
    }
}