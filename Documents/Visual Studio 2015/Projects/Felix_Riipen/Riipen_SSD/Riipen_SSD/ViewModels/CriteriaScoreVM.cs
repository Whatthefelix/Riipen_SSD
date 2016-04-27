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
        public double? Score { get; set; }
        public string Comment { get; set; }

        public CriteriaScoreVM() { }

        public CriteriaScoreVM(int CriteriaID, string Criteria, string Description, double? Score, string Comment)
        {
            this.CriteriaID = CriteriaID;
            this.Criteria = Criteria;
            this.Description = Description;
            this.Score = Score;
            this.Comment = Comment;
        }
    }
}