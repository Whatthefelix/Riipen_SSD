using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.AdminViewModels
{
    public class ContestDetailsParticipantVM
    {
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Email { get; set; }
        [Display(Name = "Team")]
        public string TeamName { get; set; }

        [Display(Name = "Website")]
        public String RiipenUrl { get; set; }
    }
}