﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DAO_ReputationService.Models
{
    public class UserReputationHistory
    {
        [Key]
        public int UserReputationHistoryID { get; set; }
        public int UserID { get; set; }
        public DateTime Date { get; set; }
        public double EarnedAmount { get; set; }
        public double LostAmount { get; set; }
        public double StakedAmount { get; set; }
        public double StakeReleasedAmount { get; set; }
        public double LastTotal { get; set; }
        public double LastStakedTotal { get; set; }
        public double LastUsableTotal { get; set; }
        public string Explanation { get; set; }
    }
}
