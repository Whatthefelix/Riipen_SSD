using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels
{
    public class TeamCriteriaScoreVM
    {

        public int TeamID { get; set; }
        public int CriteriaID { get; set; }
        public string TeamName { get; set; }
        public double? YourCurrentScore { get; set; }
        public double? FinalScore { get; set; }
        public int? JudgeNotSubmitted { get; set; }

        public TeamCriteriaScoreVM() { }
        public TeamCriteriaScoreVM(int TeamID, int CriteriaID, string TeamName,double? YourCurrentScore, double? FinalScore, int? JudgeNotSubmitted) {
            this.TeamID = TeamID;
            this.CriteriaID = CriteriaID;
            this.TeamName = TeamName;
            this.YourCurrentScore = YourCurrentScore;
            this.FinalScore = FinalScore;
            this.JudgeNotSubmitted = JudgeNotSubmitted;
        }
    }
}