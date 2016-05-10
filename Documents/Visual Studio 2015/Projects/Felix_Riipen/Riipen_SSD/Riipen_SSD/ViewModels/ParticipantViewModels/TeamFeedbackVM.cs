using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.ParticipantViewModels
{
    public class TeamFeedbackVM
    {
       [DisplayName("Judge's Name")]
       public string JudgeName { get; set; }
       [DisplayName("Public Feedback")]
       public string PublicFeedback { get; set; }
       [DisplayName("Private Feedback")]
       public string PrivateFeedback { get; set; }

        public TeamFeedbackVM() { }

        public TeamFeedbackVM(string JudgeName, string PublicFeedback, string PrivateFeedback) {
            this.JudgeName = JudgeName;
            this.PublicFeedback = PublicFeedback;
            this.PrivateFeedback = PrivateFeedback;


        }

    }
}