using Riipen_SSD.AdminViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Riipen_SSD.ViewModels
{
    public class ContestVM
    {
        [DisplayName("Contest")]
        public String ContestName { get; set; }
        public DateTime? Date { get; set; }
        public String Location { get; set; }
        public IEnumerable<ParticipantVM> Participants {get; set;}
        public IEnumerable<CriteriaVM> Criteria { get; set; }
        public IEnumerable<JudgeVM> Judges { get; set; }
    }
}
