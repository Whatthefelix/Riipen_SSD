using FileHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Riipen_SSD.AdminViewModels
{
    [DelimitedRecord(",")]
    [IgnoreFirst(1)]
    public class ParticipantVM
    {
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Email { get; set; }
        [Display(Name = "Team")]
        public string TeamName { get; set; }

    }
}