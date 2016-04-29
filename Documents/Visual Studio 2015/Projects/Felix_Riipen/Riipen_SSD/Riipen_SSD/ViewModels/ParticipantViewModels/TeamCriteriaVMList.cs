using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.ParticipantViewModels
{
    public class TeamCriteriaVMList
    {

        public List<TeamCriteriaVM> teamCriteriaVMlist = new List<TeamCriteriaVM>();
//        public bool? PubliclyViewable { get; set; }
        public List<TeamFeedbackVM> teamFeedbackVMList{ get; set; }

        public TeamCriteriaVMList() { }

        public TeamCriteriaVMList(List<TeamCriteriaVM> teamCriteriaVMlist, List<TeamFeedbackVM> teamFeedbackVMList) {
            this.teamCriteriaVMlist = teamCriteriaVMlist;
            this.teamFeedbackVMList = teamFeedbackVMList;
        }
    }
}