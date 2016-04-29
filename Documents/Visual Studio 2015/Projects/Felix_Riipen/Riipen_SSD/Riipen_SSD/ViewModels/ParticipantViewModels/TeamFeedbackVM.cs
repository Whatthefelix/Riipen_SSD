using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.ParticipantViewModels
{
    public class TeamFeedbackVM
    {
       public string JudgeName { get; set; }
       public string Feedback { get; set; }

        public TeamFeedbackVM() { }

        public TeamFeedbackVM(string JudgeName, string Feedback) {
            this.JudgeName = JudgeName;
            this.Feedback = Feedback;

        }

    }
}