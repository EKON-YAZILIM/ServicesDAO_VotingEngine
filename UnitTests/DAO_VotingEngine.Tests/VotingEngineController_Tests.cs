using System.Threading.Tasks.Dataflow;
using System;
using Xunit;
using DAO_VotingEngine.Contexts;
using DAO_VotingEngine;
using DAO_VotingEngine.Controllers;
using DAO_VotingEngine.Tests.Util;
using DAO_VotingEngine.Models;
using Helpers.Models.DtoModels.VoteDbDto;
using FluentAssertions;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Internal;
using System.Linq;

namespace DAO_VotingEngine.Tests
{

    ///<summary>
    /// Tests of DAO_VotingEngine.VoteController and DAO_VotingEngine.VotingController class methods.
    /// Methods:
    /// VoteDto Post +
    /// List<VoteDto> GetAllVotesByVotingId 
    /// SimpleResponse SubmitVote
    /// VoteDto Update
    /// VotingDto GetByJobId
    /// List<VotingDto> GetCompletedVotingsByJobIds
    /// SimpleResponse StartInformalVoting
    ///</summary>
    [Collection("Sequential")]
    public class VotingEngineController_Tests
    {
        VoteController voteController;
        VotingController votingController;

        public VotingEngineController_Tests(){
            voteController   = new VoteController();
            votingController = new VotingController();
        }       

        //VoteContrller tests
        [Fact]
        public void Post_Test(){
            //Arrange
            List<int> activeVotes = new List<int>();
            TestDbInitializer.ClearDatabase();
            Random random = new Random();
            activeVotes = TestDbInitializer.AddVotings();

            int selectedVoting = activeVotes[random.Next(1, activeVotes.Count-1)];
            VoteDto vote = new VoteDto{
                VotingID=selectedVoting,
                UserID=1,
                Date= DateTime.Now,
                Direction = Helpers.Constants.Enums.StakeType.For // For, Against, Bid, Mint              
            };
            
            //Act
            VoteDto result = voteController.Post(vote);

            //Assert
            result.VotingID.Should().Be(selectedVoting);            
        }         
        
        ///VoteController tests
        [Fact]
        public void GetAllVotesByVotingId_Test()
        {
            //Arrange
            TestDbInitializer.ClearDatabase();
            List<int> votings = TestDbInitializer.AddVotes();
            
            //Act
            var result = voteController.GetAllVotesByVotingId(votings[3]);
            
            //Assert
            foreach(VoteDto vote in result){
                vote.VotingID.Should().Be(votings[3]);
            }
        }

        [Fact]
        public void Update_Test(){
            //Arrange
            TestDbInitializer.ClearDatabase();

            //Act
            //Assert

        }

        ///VotingController Tests
        [Fact]
        public void GetByJobId_Test(){
            //Arrange
            TestDbInitializer.ClearDatabase();

            //Act

            //Assert

        }

        [Fact]
        public void GetCompletedVotingsByJobIds_Test(){

            //Arrange
            TestDbInitializer.ClearDatabase();

            
            //Act
            
            //Assert
        }

        [Fact]
        public void StartInformalVoting_Test(){
            //Arrange
            TestDbInitializer.ClearDatabase();

            //Act
            //Assert



            
        }



    }
}
