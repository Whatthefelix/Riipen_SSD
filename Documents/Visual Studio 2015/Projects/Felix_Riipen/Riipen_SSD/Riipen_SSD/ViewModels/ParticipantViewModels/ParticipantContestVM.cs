using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.ParticipantViewModels
{
    public class ParticipantContestVM
    {
        public int TeamID { get; set; }
        public int ContestID { get; set; }
        [Display(Name = "Contest Name")]
        public string ContestName { get; set; }
        [Display(Name = "Date")]
        public DateTime? StartTime { get; set; }
        public string Location { get; set; }

        public ParticipantContestVM() { }

            public ParticipantContestVM(int TeamID, int ContestID, string ContestName, DateTime? Date, string Location)
        {
            this.TeamID = TeamID;
            this.ContestID = ContestID;
            this.ContestName = ContestName;
            this.StartTime = Date;
            this.Location = Location;
        }
    }

    
}