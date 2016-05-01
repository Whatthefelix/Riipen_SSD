using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.ParticipantViewModels
{
    public class TeamFeedbackVM
    {
       public string JudgeName { get; set; }
       public string PublicFeedback { get; set; }
       public string PrivateFeedback { get; set; }

        public TeamFeedbackVM() { }

        public TeamFeedbackVM(string JudgeName, string PublicFeedback, string PrivateFeedback) {
            this.JudgeName = JudgeName;
            this.PublicFeedback = PublicFeedback;
            this.PrivateFeedback = PrivateFeedback;


        }

    }
}