using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels
{
    public class SingleJudgeCriteriaScoreVM
    {
        public List<SingleCriteriaScoreVM> singleCriteriaScoreVMLlist { get; set; }
        public string PublicFeedback { get; set; }
        public string PrivateFeedback { get; set; }

        public SingleJudgeCriteriaScoreVM() { }


        public SingleJudgeCriteriaScoreVM(List<SingleCriteriaScoreVM> singleCriteriaScoreVMLlist, string PublicFeedback, string PrivateFeedback) {

            this.singleCriteriaScoreVMLlist = singleCriteriaScoreVMLlist;
            this.PublicFeedback = PublicFeedback;
            this.PrivateFeedback = PrivateFeedback;
           
        }
    }
}