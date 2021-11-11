using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Helpers.Constants.Enums;

namespace DAO_VotingEngine.Models
{
    public class Auction 
    {
        [Key]
        public int AuctionID { get; set; }
        public int? JobID { get; set; }
        public DateTime CreateDate { get; set; }
        public int? JobPosterUserId { get; set; }
        public int? WinnerAuctionBidID { get; set; }
        public AuctionStatusTypes Status { get; set; }
        public bool? IsInternal { get; set; }
        public DateTime? PublicAuctionEndDate { get; set; }
        public DateTime? InternalAuctionEndDate { get; set; }
    }
}
