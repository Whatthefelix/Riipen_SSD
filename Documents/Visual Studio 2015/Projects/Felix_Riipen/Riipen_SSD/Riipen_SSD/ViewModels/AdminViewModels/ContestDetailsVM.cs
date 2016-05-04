using Riipen_SSD.AdminViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Riipen_SSD.AdminViewModels
{
    public class ContestDetailsVM
    {   public int ContestID { get; set; }
        public string Name { get; set; }
        [DisplayName("Start time")]
        public string StartTime { get; set; }
        public string Location { get; set; }
        public Boolean Published { get; set; }

        public IEnumerable<ParticipantVM> Participants { get; set; }
        public IEnumerable<JudgeVM> Judges { get; set; }
    }
}