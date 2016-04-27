using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.Models.ViewModels
{
    public class CriteriaScoreVM
    {
        public int CriteriaID { get; set; }
        public string Criteria { get; set; }
        public string Description { get; set; }
        public double? YourScore { get; set; }
        public double? CurrentScore { get; set; }
        public string Comment { get; set; }
        public int? JudgesNotSubmitted { get; set; }
        public bool Submitted { get; set; }

        public CriteriaScoreVM() { }

        public CriteriaScoreVM(int CriteriaID, string Criteria, string Description, double? YourScore, double? CurrentScore, string Comment, int? JudgesNotSubmitted, bool Submitted)
        {
            this.CriteriaID = CriteriaID;
            this.Criteria = Criteria;
            this.Description = Description;
            this.YourScore = YourScore;
            this.CurrentScore = CurrentScore;
            this.Comment = Comment;
            this.JudgesNotSubmitted = JudgesNotSubmitted;
            this.Submitted = Submitted;
        }
    }
}