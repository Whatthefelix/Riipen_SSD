using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.ParticipantViewModels
{
    public class ContestResultVM
    {
        public int TeamId { get; set; }
        [DisplayName("Team")]
        public string TeamName { get; set; }
        public double Score { get; set; }

        public ContestResultVM(){}

        public ContestResultVM(int TeamId, string TeamName, double Score)
        {
            this.TeamId = TeamId;
            this.TeamName = TeamName;
            this.Score = Score;


        }


       
    }
}