using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GraphQL.DataLoader.StarWarsApp.Data
{
    public class StarWarsContext : DbContext
    {
        public DbSet<Human> Humans { get; set; }
        public DbSet<Droid> Droids { get; set; }
        public DbSet<Friendship> Friendships { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./starwars.db");
        }
    }

    public class Human : ICharacter
    {
        public int HumanId { get; set; }
        public string Name { get; set; }
        public string HomePlanet { get; set; }
        public List<Friendship> Friendships { get; set; }

        public override string ToString()
        {
            return HumanId.ToString();
        }
    }

    public class Friendship
    {
        public int FriendshipId { get; set; }
        public int HumanId { get; set; }
        public int DroidId { get; set; }
        public Human Human { get; set; }
        public Droid Droid { get; set; }
    }

    public class Droid : ICharacter
    {
        public int DroidId { get; set; }
        public string Name { get; set; }
        public string PrimaryFunction { get; set; }
        public List<Friendship> Friendships { get; set; }

        public override string ToString()
        {
            return DroidId.ToString();
        } 
    }

    public interface ICharacter
    {
        string Name { get; set; }
        List<Friendship> Friendships { get; set; }
    }
}
