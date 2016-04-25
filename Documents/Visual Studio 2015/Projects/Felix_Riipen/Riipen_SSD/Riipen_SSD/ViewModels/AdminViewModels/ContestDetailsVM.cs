using Riipen_SSD.AdminViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.AdminViewModels
{
    public class ContestDetailsVM
    {
        public string Name { get; set; }
        public string StartTime { get; set; }
        public string Location { get; set; }
        public Boolean Published { get; set; }

        public IEnumerable<ParticipantVM> Participants { get; set; }
        public IEnumerable<JudgeVM> Judges { get; set; }
    }
}