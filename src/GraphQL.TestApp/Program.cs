using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using GraphQL.Execution;
using GraphQL.Http;
using GraphQL.TestApp.Data;
using GraphQL.TestApp.Schema;
using GraphQL.Types;
using Unity;

namespace GraphQL.TestApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitTestData();

            var container = new UnityContainer();
            container.RegisterInstance(new StarWarsContext());
            container.RegisterType<StarWarsQuery>();
            container.RegisterType<DroidType>();
            container.RegisterType<HumanType>();

            var schema = new StarWarsSchema(t => container.Resolve(t) as GraphType);

            var query = @" {
                droids {
                    droidId
                    name
                    primaryFunction
                    friends {
                        name
                        ... on Human {
                            humanId
                            homePlanet
                            friends {
                                friends {
                                    name
                                }
                            }
                        }
                    }
                }
            }";

            Execute(schema, query);
        }

        public static void Execute(
            ISchema schema,
            string query,
            string operationName = null,
            Inputs inputs = null)
        {
            var executer = new DocumentExecuter();
            var writer = new DocumentWriter(true);
            Console.WriteLine("Executing {0}", Regex.Replace(query, @"\s\s+", " "));
            
            var result = executer.ExecuteAsync(schema, null, query, operationName, inputs);
            FetchQueue.Execute();
            Console.WriteLine(writer.Write(result.Result));
        }

        private static void InitTestData()
        {
            using (var db = new StarWarsContext())
            {
                if (db.Humans.Any())
                    return;

                db.Humans.RemoveRange(db.Humans);
                db.Droids.RemoveRange(db.Droids);
                db.Friendships.RemoveRange(db.Friendships);

                var luke = new Human
                {
                    HumanId = 1,
                    Name = "Luke",
                    HomePlanet = "Tatooine"
                };

                var vader = new Human
                {
                    HumanId = 2,
                    Name = "Vader",
                    HomePlanet = "Tatooine"
                };
                
                var ash = new Human
                {
                    HumanId = 3,
                    Name = "Ash",
                    HomePlanet = "Cromwell"
                };

                var r2d2 = new Droid
                {
                    DroidId = 1,
                    Name = "R2-D2",
                    PrimaryFunction = "Astromech"
                };

                var c3p0 = new Droid
                {
                    DroidId = 2,
                    Name = "C-3PO",
                    PrimaryFunction = "Protocol"
                };

                db.Humans.Add(luke);
                db.Humans.Add(vader);
                db.Humans.Add(ash);
                db.Droids.Add(r2d2);
                db.Droids.Add(c3p0);

                db.Friendships.Add(new Friendship
                {
                    HumanId = luke.HumanId,
                    DroidId = r2d2.DroidId
                });

                db.Friendships.Add(new Friendship
                {
                    HumanId = luke.HumanId,
                    DroidId = c3p0.DroidId
                });

                db.Friendships.Add(new Friendship
                {
                    HumanId = vader.HumanId,
                    DroidId = r2d2.DroidId
                });

                db.Friendships.Add(new Friendship
                {
                    HumanId = ash.HumanId,
                    DroidId = c3p0.DroidId
                });

                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);
            }
        }
    }
}
