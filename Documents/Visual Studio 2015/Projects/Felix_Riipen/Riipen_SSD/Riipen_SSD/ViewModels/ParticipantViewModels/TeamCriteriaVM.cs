using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.ParticipantViewModels
{
    public class TeamCriteriaVM
    {
        public int CriteriaID { get; set; }
        public string CriteriaName { get; set; }
        public string Description { get; set; }
        public double? Score { get; set; }

        public TeamCriteriaVM() {
        }

        public TeamCriteriaVM(int CriteriaID, string CriteriaName, string Description, double? Score)
        {
            this.CriteriaID = CriteriaID;
            this.CriteriaName = CriteriaName;
            this.Description = Description;
            this.Score = Score;
        }
    }
}