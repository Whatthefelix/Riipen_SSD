using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using PagedList;

namespace Riipen_SSD.ViewModels
{
    public class TeamCriteriaScoreVM
    {
        [DisplayName("Team ID")]
        public int TeamID { get; set; }
        [DisplayName("Contest ID")]
        public int ContestID { get; set; }
        [DisplayName("Team name")]
        public string TeamName { get; set; }
        [DisplayName("Your score")]
        public double? YourCurrentScore { get; set; }
        [DisplayName("Final score")]
        public double? FinalScore { get; set; }
        [DisplayName("Status")]
        public int? JudgeNotSubmitted { get; set; }
        public bool Submitted;

        public TeamCriteriaScoreVM() { }
        public TeamCriteriaScoreVM(int TeamID, int ContestID, string TeamName,double? YourCurrentScore, double? FinalScore, int? JudgeNotSubmitted, bool Submitted) {
            this.TeamID = TeamID;
            this.ContestID = ContestID;
            this.TeamName = TeamName;
            this.YourCurrentScore = YourCurrentScore;
            this.FinalScore = FinalScore;
            this.JudgeNotSubmitted = JudgeNotSubmitted;
            this.Submitted = Submitted;
        }
    }
}