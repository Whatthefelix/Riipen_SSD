using Riipen_SSD.AdminViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Riipen_SSD.ViewModels
{
    public class ContestVM
    {

        [DisplayName("Contest")]
        public String ContestName { get; set; }
        [DisplayName("Date")]
        public DateTime? StartTime { get; set; }
        public String Location { get; set; }
        public IEnumerable<ParticipantVM> Participants {get; set;}
        public IEnumerable<CriteriaVM> Criteria { get; set; }
        public IEnumerable<JudgeVM> Judges { get; set; }
    }
}
