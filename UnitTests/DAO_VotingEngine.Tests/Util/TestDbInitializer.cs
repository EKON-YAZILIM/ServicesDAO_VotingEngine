using Microsoft.Extensions.Configuration;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using PagedList.Core;
using System.ComponentModel;
using System.Linq;
using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Org.BouncyCastle.Math.EC.Multiplier;
using DAO_VotingEngine.Contexts;
using DAO_VotingEngine;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using Helpers.Models.DtoModels.VoteDbDto;
using DAO_VotingEngine.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;

namespace DAO_VotingEngine.Tests.Util
{

    /// <summary>
    /// TestDbInitializer class includes necessary environment mocking methods.
    /// - Initializes database, creates application dbcontext
    /// - Creates testing environment
    /// </summary>
    public static class TestDbInitializer
    {
        ///Test password generated for user model crud operations tests
        public static string testPassword = Guid.NewGuid().ToString("d").Substring(1, 6);
        static dao_votesdb_context context;
        /// <summary>
        /// Creates a DB context which should use a "test DB instance".
        /// Deletes the database.
        /// Builds a database and its entity tables using the DB context        
        /// </summary>
        static TestDbInitializer()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            DAO_VotingEngine.Startup.LoadConfig(config);

            context = new dao_votesdb_context();
            context.ChangeTracker.Clear();
            context.Database.EnsureDeleted();
            DAO_VotingEngine.Startup.InitializeService();
        }

        /// <summary>
        /// Truncates Votes, Votings tables in the database
        /// </summary>
        public static void ClearDatabase()
        {
            foreach (var vote in context.Votes)
            {
                context.Remove(vote);
                context.Entry(vote).State = EntityState.Detached;
            }
            foreach (var voting in context.Votings)
            {
                context.Remove(voting);
                context.Entry(voting).State = EntityState.Detached;
            }
            
            context.SaveChanges();
            context.ChangeTracker.Clear();
        }

        ///<summary>
        /// Adds votings to database and returns the list of votings Ids
        ///</summary>
        ///<returns>List<int></returns>
        public static List<int> AddVotings(){
            ClearDatabase();
            List<int> votingIds = new List<int>();

            Random rand = new Random();
            for (int i = 0; i<rand.Next(8,20); i++){
                
                int createGap = rand.Next(20, 25);

                DateTime crDate = DateTime.Now.AddDays(-createGap);

                Voting voting = new Voting{
                    JobID         = i,
                    IsFormal      = false,
                    CreateDate    = crDate,
                    StartDate     = crDate.AddDays(4),
                    EndDate       = crDate.AddDays(19),
                    Status        = Helpers.Constants.Enums.VoteStatusTypes.Active, //VoteStatusTypes Pending, Active, Waiting, Completed, Expired
                    QuorumCount   = 0,
                    Type          = Helpers.Constants.Enums.VoteTypes.Governance, //VoteTypes Simple, Governance, Admin, JobCompletion, JobRefund
                    VoteCount     = 0, //
                    QuorumRatio   = 0,
                    StakedFor     = 0,
                    StakedAgainst = 0,
                    //How much of the new minted reputation will be distributed to job doer..
                    PolicingRate  = 0.5
                };

                context.Votings.Add(voting);
                context.SaveChanges();
                votingIds.Add(voting.VotingID);                
            }           

            return votingIds;
        }
        
        ///<summary>
        /// Clears the database.
        /// Creates a voting environment with active votings
        ///</summary>        
        public static List<int> AddVotes(){
            ClearDatabase();
            List<int> votingIds = AddVotings();
            Random rand = new Random();

            for (int i =0; i < rand.Next(20, 190); i++){

                Vote vote = new Vote{
                    VotingID  = votingIds[rand.Next(1, votingIds.Count)],
                    UserID    = rand.Next(1,50),
                    Date      = DateTime.Now,
                    Direction = Helpers.Constants.Enums.StakeType.For // For, Against, Bid, Mint              
                };
                context.Votes.Add(vote);
            }

            for (int j =0; j < rand.Next(20, 190); j++){

                Vote vote = new Vote{
                    VotingID  = votingIds[rand.Next(1, votingIds.Count)],
                    UserID    = rand.Next(1,50),
                    Date      = DateTime.Now.AddDays(-rand.Next(2,15)),
                    Direction = Helpers.Constants.Enums.StakeType.Against // For, Against, Bid, Mint              
                };
                context.Votes.Add(vote);
            }
            context.SaveChanges();
            return votingIds;
        }
    }
}