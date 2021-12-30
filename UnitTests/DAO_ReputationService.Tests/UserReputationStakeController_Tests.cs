using System;
using Xunit;
using DAO_ReputationService.Models;
using Helpers.Models.DtoModels.ReputationDbDto;
using DAO_ReputationService.Controllers;
using FluentAssertions;
using DAO_ReputationService.Tests.Util;
using Microsoft.Extensions.Configuration;

namespace DAO_ReputationService.Tests
{
    [Collection("Sequential")]
    public class UserReputationStakeController_Tests
    {

        UserReputationStakeController stakecontroller;
        UserReputationHistoryController stakehistorycontroller;
        public UserReputationStakeController_Tests(){
            stakecontroller = new UserReputationStakeController();
            stakehistorycontroller = new UserReputationHistoryController();
        }
        
        ///UserReputationStakeDto Post([FromBody] UserReputationStakeDto model)
        [Fact]
        public void Post_Test()
        {
            TestDbInitializer.ClearDatabase();
            Random random = new Random();
            //Arrange
            for(int i = 0;i<random.Next(4,33);i++){
                UserReputationStakeDto userReputationStake = new UserReputationStakeDto{
                    UserID = 1,
                    CreateDate = DateTime.Now.AddDays(-365 + i^2/4),
                    ReferenceID = i%3, //Id of the bid or the vote (can be anything else), ReputationStakeTypes indicates the type of this stake
                    ReferenceProcessID = i%6, //Id of the auction or the voting (can be anything else), indicates the process which this stake belongs to
                    Amount = 150*(i%8),
                    Type = Helpers.Constants.Enums.StakeType.For, //Against, For, Mint, Bid
                    Status = Helpers.Constants.Enums.ReputationStakeStatus.Staked //Staked, Released
                };
                //Act
                UserReputationStakeDto result = stakecontroller.Post(userReputationStake);
                //Assert
                result.Should().Be(result);
            }
        }

        ///UserReputationStakeDto Post([FromBody] UserReputationStakeDto model)
        [Fact]
        public void SubmitStake_Test(){


        }

        ///UserReputationStakeDto Post([FromBody] UserReputationStakeDto model)
        [Fact]
        public void ReleaseSingleStake_Test(){

        }

        ///UserReputationStakeDto Post([FromBody] UserReputationStakeDto model)
        [Fact]
        public void DistributeStakes_Test(){

        }

        ///UserReputationStakeDto Post([FromBody] UserReputationStakeDto model)
        [Fact]
        public void GetLastReputation_Test(){

        }
    }
}
