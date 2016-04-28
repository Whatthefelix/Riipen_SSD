using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.ParticipantViewModels
{
    public class CriteriaDetailVM
    {
        public double? Score { get; set; }
        public string JudgeName { get; set; }
        public string Comment { get; set; }

        public CriteriaDetailVM() { }

        public CriteriaDetailVM(double? Score, string JudgeName, string Comment) {
            this.Score = Score;
            this.JudgeName = JudgeName;
            this.Comment = Comment;
        }
    }
}