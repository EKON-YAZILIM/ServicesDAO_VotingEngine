﻿// <auto-generated />
using System;
using DAO_VotingEngine.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAO_VotingEngine.Migrations
{
    [DbContext(typeof(dao_votesdb_context))]
    partial class dao_votesdb_contextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("DAO_VotingEngine.Models.Vote", b =>
                {
                    b.Property<int>("VoteID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime");

                    b.Property<int>("Direction")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.Property<int>("VotingID")
                        .HasColumnType("int");

                    b.HasKey("VoteID");

                    b.ToTable("Votes");
                });

            modelBuilder.Entity("DAO_VotingEngine.Models.Voting", b =>
                {
                    b.Property<int>("VotingID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime");

                    b.Property<bool>("IsFormal")
                        .HasColumnType("tinyint(1)");

                    b.Property<int>("JobID")
                        .HasColumnType("int");

                    b.Property<double>("PolicingRate")
                        .HasColumnType("double");

                    b.Property<int?>("QuorumCount")
                        .HasColumnType("int");

                    b.Property<double?>("StakedAgainst")
                        .HasColumnType("double");

                    b.Property<double?>("StakedFor")
                        .HasColumnType("double");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<int>("VoteCount")
                        .HasColumnType("int");

                    b.HasKey("VotingID");

                    b.ToTable("Votings");
                });
#pragma warning restore 612, 618
        }
    }
}
