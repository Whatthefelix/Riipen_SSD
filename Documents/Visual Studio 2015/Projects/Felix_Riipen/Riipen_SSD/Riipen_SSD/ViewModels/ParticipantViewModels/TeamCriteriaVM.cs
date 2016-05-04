using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.ParticipantViewModels
{
    public class TeamCriteriaVM
    {
        [DisplayName("Criteria ID")]
        public int CriteriaID { get; set; }
        [Display(Name = "Criteria")]
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