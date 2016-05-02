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

        public int TeamID { get; set; }
        public int ContestID { get; set; }
        public string TeamName { get; set; }
        public double? YourCurrentScore { get; set; }
        public double? FinalScore { get; set; }
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