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
        [Display(Name = "First Name")]
        public string FirstName;
        [Display(Name = "Last Name")]
        public string LastName;
        public string Email;
        [Display(Name = "Team")]
        public string TeamName;

        [FieldOptional]
        [Display(Name = "Website")]
        public String RiipenUrl;

    }
}