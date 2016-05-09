using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels.ParticipantViewModels
{
    public class TeamMemberVM
    {
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }

        public string Email { get; set; }

        public TeamMemberVM() { }

        public TeamMemberVM(string FirstName, string LastName, string Email)
        {
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Email = Email;
        }

    }
}