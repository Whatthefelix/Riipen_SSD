using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels
{
    public class SingleJudgeCriteriaScoreVM
    {
        public List<SingleCriteriaScoreVM> singleCriteriaScoreVMLlist { get; set; }
        [DisplayName("Public feedback")]
        public string PublicFeedback { get; set; }
        [DisplayName("Private feedback")]
        public string PrivateFeedback { get; set; }

        public SingleJudgeCriteriaScoreVM() { }


        public SingleJudgeCriteriaScoreVM(List<SingleCriteriaScoreVM> singleCriteriaScoreVMLlist, string PublicFeedback, string PrivateFeedback) {
            this.singleCriteriaScoreVMLlist = singleCriteriaScoreVMLlist;
            this.PublicFeedback = PublicFeedback;
            this.PrivateFeedback = PrivateFeedback;
        }
    }
}