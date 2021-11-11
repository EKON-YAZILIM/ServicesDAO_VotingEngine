using DAO_VotingEngine.Contexts;
using DAO_VotingEngine.Models;
using Helpers.Constants;
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
                        db.Entry(auction).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        db.SaveChanges();
                    }


                    //Check if auction public bidding ended without any winner -> Set auction status to Expired
                    var ongoingAuctions = db.Auctions.Where(x => x.Status == Enums.AuctionStatusTypes.PublicBidding && x.PublicAuctionEndDate < DateTime.Now).ToList();

                    foreach (var auction in ongoingAuctions)
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
                    //Check if informal voting ended -> Start formal voting if quorum reached, else set voting status to Expired
                    var informalVotings = db.Votings.Where(x => x.IsFormal == false && x.EndDate < DateTime.Now).ToList();

                    foreach (var vote in informalVotings)
                    {                     
                        //
                        vote.Status = Enums.VoteStatusTypes.Completed;
                        db.Entry(vote).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        db.SaveChanges();
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
