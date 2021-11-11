using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_VotingEngine.Models
{
    public class Vote
    {
        [Key]
        public int VoteID { get; set; }
        public int VotingID { get; set; }
        public int UserID { get; set; }
        public double Reputation { get; set; }
        public string Side { get; set; }
        public DateTime Date { get; set; }

    }
}
