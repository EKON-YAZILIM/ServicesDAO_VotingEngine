using DAO_VotingEngine.Contexts;
using DAO_VotingEngine.Models;
using Helpers.Constants;
using Helpers.Models.DtoModels.ReputationDbDto;
using Helpers.Models.SharedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace DAO_VotingEngine
{
    public class TimerEvents
    {
        //Auction status control timer
        public static System.Timers.Timer auctionStatusTimer;
        //Voting status control timer
        public static System.Timers.Timer votingStatusTimer;

        /// <summary>
        ///  Start timer controls of the application
        /// </summary>
        public static void StartTimers()
        {
            CheckAuctionStatus(null, null);
            CheckVotingStatus(null, null);

            //Auction status timer
            auctionStatusTimer = new System.Timers.Timer(10000);
            auctionStatusTimer.Elapsed += CheckAuctionStatus;
            auctionStatusTimer.AutoReset = true;
            auctionStatusTimer.Enabled = true;

            //Voting status timer
            votingStatusTimer = new System.Timers.Timer(10000);
            votingStatusTimer.Elapsed += CheckVotingStatus;
            votingStatusTimer.AutoReset = true;
            votingStatusTimer.Enabled = true;
        }

        /// <summary>
        ///  Check auction status 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void CheckAuctionStatus(Object source, ElapsedEventArgs e)
        {
            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    //Check if auction internal bidding ended -> Start public bidding
                    var publicAuctions = db.Auctions.Where(x => x.Status == Enums.AuctionStatusTypes.InternalBidding && x.InternalAuctionEndDate < DateTime.Now && x.WinnerAuctionBidID == null).ToList();

                    foreach (var auction in publicAuctions)
                    {
                        auction.Status = Enums.AuctionStatusTypes.PublicBidding;
                        auction.PublicAuctionEndDate = DateTime.Now.AddDays(Program._settings.PublicAuctionDays);
                        db.Entry(auction).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        db.SaveChanges();
                    }


                    //Check if auction public bidding ended without any winner -> Set auction status to Expired
                    var expiredAuctions = db.Auctions.Where(x => x.Status == Enums.AuctionStatusTypes.PublicBidding && x.PublicAuctionEndDate < DateTime.Now).ToList();

                    foreach (var auction in expiredAuctions)
                    {
                        //No winners selected. Auction expired.
                        if (auction.WinnerAuctionBidID == null)
                        {
                            auction.Status = Enums.AuctionStatusTypes.Expired;
                            db.Entry(auction).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }


                    //Check if a winner bid selected for an auction -> Set auction status to Completed and start informal voting
                    var completedAuctions = db.Auctions.Where(x => x.WinnerAuctionBidID != null && (x.Status == Enums.AuctionStatusTypes.PublicBidding || x.Status == Enums.AuctionStatusTypes.InternalBidding)).ToList();

                    foreach (var auction in completedAuctions)
                    {
                        auction.Status = Enums.AuctionStatusTypes.Completed;
                        db.Entry(auction).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        db.SaveChanges();

                        //Start informal voting
                        Voting voting = new Voting();
                        voting.CreateDate = DateTime.Now;
                        voting.StartDate = DateTime.Now;
                        voting.EndDate = DateTime.Now.AddDays(Program._settings.InformalVotingDays);
                        voting.IsFormal = false;
                        voting.JobID = Convert.ToInt32(auction.JobID);
                        voting.Status = Enums.VoteStatusTypes.Active;
                        voting.Type = Enums.VoteTypes.JobCompletion;

                        //Set quorum count based on DAO member count
                        voting.QuorumCount = Convert.ToInt32(Convert.ToDouble(auction.DAOMemberCount) * Program._settings.QuorumRatio);
                        db.Votings.Add(voting);
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Program.monitizer.AddConsole("Exception in timer CheckAuctionStatus. Ex: " + ex.Message);
            }
        }

        /// <summary>
        ///  Check voting status
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private static void CheckVotingStatus(Object source, ElapsedEventArgs e)
        {
            try
            {
                using (dao_votesdb_context db = new dao_votesdb_context())
                {
                    //Get ended informal votings -> Start formal voting if quorum reached, else set voting status to Expired
                    var informalVotings = db.Votings.Where(x => x.IsFormal == false && x.EndDate < DateTime.Now).ToList();

                    foreach (var voting in informalVotings)
                    {
                        //Check if quorum is reached
                        var votes = db.Votes.Where(x => x.VotingID == voting.VotingID);
                        //Quorum reached -> Start formal voting
                        if (voting.QuorumCount != null && votes.Count() >= Convert.ToInt32(voting.QuorumCount))
                        {
                            voting.Status = Enums.VoteStatusTypes.Completed;
                            db.Entry(voting).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            db.SaveChanges();

                            //Start formal voting
                            Voting formalVoting = new Voting();
                            formalVoting.CreateDate = DateTime.Now;
                            formalVoting.StartDate = DateTime.Now;
                            formalVoting.EndDate = DateTime.Now.AddDays(Program._settings.FormalVotingDays);
                            formalVoting.IsFormal = true;
                            formalVoting.JobID = Convert.ToInt32(voting.JobID);
                            formalVoting.Status = Enums.VoteStatusTypes.Active;
                            formalVoting.Type = voting.Type;

                            //Set quorum count based on DAO member count
                            formalVoting.QuorumCount = voting.QuorumCount;
                            db.Votings.Add(formalVoting);
                            db.SaveChanges();
                        }
                        //Quorum isn't reached -> Set voting status to Expired
                        else
                        {
                            voting.Status = Enums.VoteStatusTypes.Expired;
                            db.Entry(voting).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            db.SaveChanges();
                        }

                        //Release staked reputations
                        var jsonResult = Helpers.Request.Get(Program._settings.Service_Reputation_Url + "/UserReputationStake/ReleaseStakes?referenceProcessID=" + voting.VotingID);
                        SimpleResponse parsedResult = Helpers.Serializers.DeserializeJson<SimpleResponse>(jsonResult);
                    }

                    //Get ended formal votings -> Distribute or release reputations
                    var formalVotings = db.Votings.Where(x => x.IsFormal == true && x.EndDate < DateTime.Now).ToList();

                    foreach (var voting in formalVotings)
                    {
                        //Check if quorum is reached
                        var votes = db.Votes.Where(x => x.VotingID == voting.VotingID);
                        //Quorum reached -> Start formal voting
                        if (voting.QuorumCount != null && votes.Count() >= Convert.ToInt32(voting.QuorumCount))
                        {
                            voting.Status = Enums.VoteStatusTypes.Completed;
                            db.Entry(voting).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            db.SaveChanges();

                            //Get total reputation stakes for this vote
                            var stakesJson = Helpers.Request.Get(Program._settings.Service_Reputation_Url + "/UserReputationStake/GetByProcessId?referenceProcessID=" + voting.VotingID);
                            List<UserReputationStakeDto> parsedStakes = Helpers.Serializers.DeserializeJson<List<UserReputationStakeDto>>(stakesJson);

                            //Find winning side
                            Enums.VoteDirection winnerSide = Enums.VoteDirection.For;
                            double forReps = parsedStakes.Where(x => x.Direction == Enums.VoteDirection.For).Sum(x => x.Amount);
                            double againstReps = parsedStakes.Where(x => x.Direction == Enums.VoteDirection.Against).Sum(x => x.Amount);
                            if (againstReps > forReps)
                            {
                                winnerSide = Enums.VoteDirection.Against;
                            }

                            //Distribute staked reputations
                            var jsonResult = Helpers.Request.Get(Program._settings.Service_Reputation_Url + "/UserReputationStake/DistributeStakes?referenceProcessID=" + voting.VotingID + "&winnerDirection=" + winnerSide);
                            SimpleResponse parsedResult = Helpers.Serializers.DeserializeJson<SimpleResponse>(jsonResult);
                        }
                        //Quorum isn't reached -> Set voting status to Expired
                        else
                        {
                            voting.Status = Enums.VoteStatusTypes.Expired;
                            db.Entry(voting).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                            db.SaveChanges();

                            //Release staked reputations
                            var jsonResult = Helpers.Request.Get(Program._settings.Service_Reputation_Url + "/UserReputationStake/ReleaseStakes?referenceProcessID=" + voting.VotingID);
                            SimpleResponse parsedResult = Helpers.Serializers.DeserializeJson<SimpleResponse>(jsonResult);
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Program.monitizer.AddConsole("Exception in timer CheckVotingStatus. Ex: " + ex.Message);
            }
        }
    }
}
