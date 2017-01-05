using System.Diagnostics;
using DataLoader.Core;
using GraphQL.DataLoader.StarWars.Schema;
using Microsoft.AspNetCore.Mvc;

namespace GraphQL.DataLoader.StarWars.Controllers
{
    [Route("api/graphql")]
    public class GraphQLController : Controller
    {
        private DocumentExecuter _executer = new DocumentExecuter();
        private StarWarsSchema _schema = new StarWarsSchema();

        [HttpPost]
        public async Task<ExecutionResult> Post([FromBody] GraphQLRequest request)
        {
            var watch = Stopwatch.StartNew();
            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - Executing GraphQL query");

            var result = await DataLoaderContext.Run(loadCtx =>
                _executer.ExecuteAsync(_schema, null, request.Query, null, userContext: new GraphQLUserContext { LoadContext = loadCtx }));

            Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId} - Finished query ({watch.ElapsedMilliseconds}ms)");

            return result;
        }
    }

    public class GraphQLRequest
    {
        public string Query { get; set; }
        public object Variables { get; set; }
    }
}