using System.Linq;
using GraphQL.DataLoader.StarWars.Schema;
using Microsoft.AspNetCore.Hosting;

namespace GraphQL.DataLoader.StarWars
{
    public class Program
    {
        public static void Main(string[] args)
        {
            InitTestData();

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
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
                db.Humans.Add(luke);

                var vader = new Human
                {
                    HumanId = 2,
                    Name = "Vader",
                    HomePlanet = "Tatooine"
                };
                db.Humans.Add(vader);

                var ash = new Human
                {
                    HumanId = 3,
                    Name = "Ash",
                    HomePlanet = "Cromwell"
                };
                db.Humans.Add(ash);

                var leia = new Human
                {
                    HumanId = 4,
                    Name = "Leia",
                    HomePlanet = "Tatooine"
                };
                db.Humans.Add(leia);

                var george = new Human
                {
                    HumanId = 5,
                    Name = "George",
                    HomePlanet = null
                };
                db.Humans.Add(george);

                var r2d2 = new Droid
                {
                    DroidId = 1,
                    Name = "R2-D2",
                    PrimaryFunction = "Astromech"
                };
                db.Droids.Add(r2d2);

                var c3p0 = new Droid
                {
                    DroidId = 2,
                    Name = "C-3PO",
                    PrimaryFunction = "Protocol"
                };
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

                var newHope = new Episode
                {
                    EpisodeId = (int)Episodes.NEWHOPE,
                    Name = "A New Hope",
                    Year = null
                };
                db.Episodes.Add(newHope);

                var empire = new Episode
                {
                    EpisodeId = (int)Episodes.EMPIRE,
                    Name = "Rise of the Empire",
                    Year = "1980"
                };
                db.Episodes.Add(empire);

                var jedi = new Episode
                {
                    EpisodeId = (int)Episodes.JEDI,
                    Name = "Rise of the Jedi",
                    Year = "1983"
                };
                db.Episodes.Add(jedi);

                db.HumanAppearances.Add(new HumanAppearance
                {
                    HumanId = ash.HumanId,
                    EpisodeId = (int)Episodes.EMPIRE
                });

                db.HumanAppearances.Add(new HumanAppearance
                {
                    HumanId = vader.HumanId,
                    EpisodeId = (int) Episodes.EMPIRE
                });

                db.DroidAppearances.Add(new DroidAppearance
                {
                    DroidId = r2d2.DroidId,
                    EpisodeId = (int)Episodes.EMPIRE
                });

                db.DroidAppearances.Add(new DroidAppearance
                {
                    DroidId = r2d2.DroidId,
                    EpisodeId = (int)Episodes.NEWHOPE
                });

                db.DroidAppearances.Add(new DroidAppearance
                {
                    DroidId = r2d2.DroidId,
                    EpisodeId = (int)Episodes.JEDI
                });

                db.DroidAppearances.Add(new DroidAppearance
                {
                    DroidId = c3p0.DroidId,
                    EpisodeId = (int)Episodes.NEWHOPE
                });

                var count = db.SaveChanges();
                Console.WriteLine("{0} records saved to database", count);
            }
        }
    }
}