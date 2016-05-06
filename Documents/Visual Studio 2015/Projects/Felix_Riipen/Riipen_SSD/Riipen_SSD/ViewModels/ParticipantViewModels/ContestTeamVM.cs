using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.ParticipantViewModels
{
    public class ContestTeamVM
    {
        [DisplayName("Team ID")]
        public int TeamID { get; set; }
        [DisplayName("Team")]
        public string TeamName { get; set; }
        public double? Score { get; set; }
        [DisplayName("Status")]
        public int? JudgeNotSubmitted { get; set; }
        public List<string> NamesOfUnsubmittedJudges { get; set; }

        public ContestTeamVM() { }
        public ContestTeamVM(int TeamID, string TeamName, double? Score, int? JudgeNotSubmitted, List<string> NamesOfUnsubmittedJudges) {
            this.TeamID = TeamID;
            this.TeamName = TeamName;
            this.Score = Score;
            this.JudgeNotSubmitted = JudgeNotSubmitted;
            this.NamesOfUnsubmittedJudges = NamesOfUnsubmittedJudges;
        }
    }
}