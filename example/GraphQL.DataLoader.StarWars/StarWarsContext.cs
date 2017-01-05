using Microsoft.EntityFrameworkCore;

namespace GraphQL.DataLoader.StarWars
{
    public class StarWarsContext : DbContext
    {
        public DbSet<Human> Humans { get; set; }
        public DbSet<Droid> Droids { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Episode> Episodes { get; set; }
        public DbSet<DroidAppearance> DroidAppearances { get; set; }
        public DbSet<HumanAppearance> HumanAppearances { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./starwars.db");
        }
    }

    public interface ICharacter
    {
        string Name { get; set; }
        List<Friendship> Friendships { get; set; }
    }

    public class Human : ICharacter
    {
        public int HumanId { get; set; }
        public string Name { get; set; }
        public string HomePlanet { get; set; }
        public List<Friendship> Friendships { get; set; }
        public List<HumanAppearance> Appearances { get; set; }

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
        public List<DroidAppearance> Appearances { get; set; }

        public override string ToString()
        {
            return DroidId.ToString();
        } 
    }

    public interface ICharacterAppearance
    {
        int EpisodeId { get; }
        Episode Episode { get; }
        int CharacterId { get; }
        ICharacter Character { get; }
    }

    public class DroidAppearance : ICharacterAppearance
    {
        public int DroidAppearanceId { get; set; }
        public int EpisodeId { get; set; }
        public Episode Episode { get; set; }
        public int DroidId { get; set; }
        public Droid Droid { get; set; }
        int ICharacterAppearance.CharacterId { get { return DroidId; } }
        ICharacter ICharacterAppearance.Character { get { return Droid; } }
    }

    public class HumanAppearance : ICharacterAppearance
    {
        public int HumanAppearanceId { get; set; }
        public int EpisodeId { get; set; }
        public Episode Episode { get; set; }
        public int HumanId { get; set; }
        public Human Human { get; set; }
        int ICharacterAppearance.CharacterId { get { return HumanId; } }
        ICharacter ICharacterAppearance.Character { get { return Human; } }
    }

    public class Episode
    {
        public int EpisodeId { get; set; }
        public string Name { get; set; }
        public string Year { get; set; }
        public List<DroidAppearance> DroidAppearances { get; set; }
        public List<HumanAppearance> HumanAppearances { get; set; }
    }
}
