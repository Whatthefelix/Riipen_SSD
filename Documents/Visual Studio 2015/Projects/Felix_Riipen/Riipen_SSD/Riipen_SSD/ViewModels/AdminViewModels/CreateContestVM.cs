using Riipen_SSD.AdminViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.AdminViewModels
{
    public class CreateContestVM
    {
        [Display (Name = "Contest")]
        [Required]
        public String ContestName { get; set; }
        [Display(Name = "Date")]
        public DateTime? StartTime { get; set; }
        public String Location { get; set; }
        [Required]
        public bool PubliclyViewable { get; set; }
        public IEnumerable<ParticipantVM> Participants { get; set; }
        public IEnumerable<CriteriaVM> Criteria { get; set; }
        public IEnumerable<JudgeVM> Judges { get; set; }
    }
}