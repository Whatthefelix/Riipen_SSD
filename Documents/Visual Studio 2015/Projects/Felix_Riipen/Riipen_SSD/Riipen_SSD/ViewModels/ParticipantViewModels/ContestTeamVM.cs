﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.ParticipantViewModels
{
    public class ContestTeamVM
    {
        public int TeamID { get; set; } 
        public string TeamName { get; set; }
        public double? Score { get; set; }
        public int? JudgeNotSubmitted { get; set; }

        public ContestTeamVM() { }
        public ContestTeamVM(int TeamID, string TeamName, double? Score, int? JudgeNotSubmitted) {
            this.TeamID = TeamID;
            this.TeamName = TeamName;
            this.Score = Score;
            this.JudgeNotSubmitted = JudgeNotSubmitted;
        }
    }
}