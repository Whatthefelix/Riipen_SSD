using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels
{
    public class SingleJudgeCriteriaScoreVM
    {
        public List<SingleCriteriaScoreVM> singleCriteriaScoreVMLlist { get; set; }
        public string Feedback { get; set; }


        public SingleJudgeCriteriaScoreVM() { }


        public SingleJudgeCriteriaScoreVM(List<SingleCriteriaScoreVM> singleCriteriaScoreVMLlist, string Feedback) {

            this.singleCriteriaScoreVMLlist = singleCriteriaScoreVMLlist;
            this.Feedback = Feedback;
           
        }
    }
}