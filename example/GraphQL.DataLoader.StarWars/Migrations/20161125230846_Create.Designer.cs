using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GraphQL.DataLoader.StarWars.Migrations
{
    [DbContext(typeof(StarWarsContext))]
    [Migration("20161125230846_Create")]
    partial class Create
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("DataLoader.StarWars.Data.Droid", b =>
                {
                    b.Property<int>("DroidId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("PrimaryFunction");

                    b.HasKey("DroidId");

                    b.ToTable("Droids");
                });

            modelBuilder.Entity("DataLoader.StarWars.Data.DroidAppearance", b =>
                {
                    b.Property<int>("DroidAppearanceId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DroidId");

                    b.Property<int>("EpisodeId");

                    b.HasKey("DroidAppearanceId");

                    b.HasIndex("DroidId");

                    b.HasIndex("EpisodeId");

                    b.ToTable("DroidAppearances");
                });

            modelBuilder.Entity("DataLoader.StarWars.Data.Episode", b =>
                {
                    b.Property<int>("EpisodeId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("Year");

                    b.HasKey("EpisodeId");

                    b.ToTable("Episodes");
                });

            modelBuilder.Entity("DataLoader.StarWars.Data.Friendship", b =>
                {
                    b.Property<int>("FriendshipId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DroidId");

                    b.Property<int>("HumanId");

                    b.HasKey("FriendshipId");

                    b.HasIndex("DroidId");

                    b.HasIndex("HumanId");

                    b.ToTable("Friendships");
                });

            modelBuilder.Entity("DataLoader.StarWars.Data.Human", b =>
                {
                    b.Property<int>("HumanId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("HomePlanet");

                    b.Property<string>("Name");

                    b.HasKey("HumanId");

                    b.ToTable("Humans");
                });

            modelBuilder.Entity("DataLoader.StarWars.Data.HumanAppearance", b =>
                {
                    b.Property<int>("HumanAppearanceId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EpisodeId");

                    b.Property<int>("HumanId");

                    b.HasKey("HumanAppearanceId");

                    b.HasIndex("EpisodeId");

                    b.HasIndex("HumanId");

                    b.ToTable("HumanAppearances");
                });

            modelBuilder.Entity("DataLoader.StarWars.Data.DroidAppearance", b =>
                {
                    b.HasOne("DataLoader.StarWars.Data.Droid", "Droid")
                        .WithMany("Appearances")
                        .HasForeignKey("DroidId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataLoader.StarWars.Data.Episode", "Episode")
                        .WithMany("DroidAppearances")
                        .HasForeignKey("EpisodeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataLoader.StarWars.Data.Friendship", b =>
                {
                    b.HasOne("DataLoader.StarWars.Data.Droid", "Droid")
                        .WithMany("Friendships")
                        .HasForeignKey("DroidId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataLoader.StarWars.Data.Human", "Human")
                        .WithMany("Friendships")
                        .HasForeignKey("HumanId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DataLoader.StarWars.Data.HumanAppearance", b =>
                {
                    b.HasOne("DataLoader.StarWars.Data.Episode", "Episode")
                        .WithMany("HumanAppearances")
                        .HasForeignKey("EpisodeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DataLoader.StarWars.Data.Human", "Human")
                        .WithMany("Appearances")
                        .HasForeignKey("HumanId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
