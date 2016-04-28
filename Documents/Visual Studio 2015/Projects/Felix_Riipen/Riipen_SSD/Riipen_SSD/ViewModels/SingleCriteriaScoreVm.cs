using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels
{
    public class SingleCriteriaScoreVM
    {
        public int CriteriaID { get; set; }
        public string CriteriaName { get; set; }
        public string Description { get; set; }
        public double? Score { get; set; }
        public string Comment { get; set; }

        public SingleCriteriaScoreVM() { }

        public SingleCriteriaScoreVM(int CriteriaID, string CriteriaName, string Description, double? Score, string Comment) {

            this.CriteriaID = CriteriaID;
            this.CriteriaName = CriteriaName;
            this.Description = Description;
            this.Score = Score;
            this.Comment = Comment;
        }


    }
}