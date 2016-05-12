using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.AdminViewModels
{
    public class TeamScoreVM
    {
        public int? TeamID { get; set; }
        [DisplayName("Team name")]
        public String TeamName { get; set; }
        [DisplayName("Final score")]
        public double? FinalScore { get; set; }

        public TeamScoreVM() { }

        public TeamScoreVM(int? TeamId, String TeamName, double? FinalScore)
        {
            this.TeamID = TeamId;
            this.TeamName = TeamName;
            this.FinalScore = FinalScore;
        }
                
    }
}