using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.ParticipantViewModels
{
    public class TeamCriteriaVMList
    {

        public List<TeamCriteriaVM> teamCriteriaVMlist = new List<TeamCriteriaVM>();
        public bool? PubliclyViewable { get; set; }
        public string Feedback { get; set; }

        public TeamCriteriaVMList() { }

        public TeamCriteriaVMList(List<TeamCriteriaVM> teamCriteriaVMlist, bool? PubliclyViewable, string Feedback) {
            this.teamCriteriaVMlist = teamCriteriaVMlist;
            this.PubliclyViewable = PubliclyViewable;
            this.Feedback = Feedback;
        }
    }
}