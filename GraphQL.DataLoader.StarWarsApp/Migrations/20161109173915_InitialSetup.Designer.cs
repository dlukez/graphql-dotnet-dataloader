using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using GraphQL.DataLoader.StarWarsApp.Data;

namespace GraphQL.DataLoader.StarWarsApp.Migrations
{
    [DbContext(typeof(StarWarsContext))]
    [Migration("20161109173915_InitialSetup")]
    partial class InitialSetup
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1");

            modelBuilder.Entity("GraphQL.DataLoader.StarWarsApp.Data.Droid", b =>
                {
                    b.Property<int>("DroidId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("PrimaryFunction");

                    b.HasKey("DroidId");

                    b.ToTable("Droids");
                });

            modelBuilder.Entity("GraphQL.DataLoader.StarWarsApp.Data.Friendship", b =>
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

            modelBuilder.Entity("GraphQL.DataLoader.StarWarsApp.Data.Human", b =>
                {
                    b.Property<int>("HumanId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("HomePlanet");

                    b.Property<string>("Name");

                    b.HasKey("HumanId");

                    b.ToTable("Humans");
                });

            modelBuilder.Entity("GraphQL.DataLoader.StarWarsApp.Data.Friendship", b =>
                {
                    b.HasOne("GraphQL.DataLoader.StarWarsApp.Data.Droid", "Droid")
                        .WithMany("Friendships")
                        .HasForeignKey("DroidId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("GraphQL.DataLoader.StarWarsApp.Data.Human", "Human")
                        .WithMany("Friendships")
                        .HasForeignKey("HumanId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
