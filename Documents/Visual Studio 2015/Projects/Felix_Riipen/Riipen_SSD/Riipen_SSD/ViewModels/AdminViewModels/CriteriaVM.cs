using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Riipen_SSD.AdminViewModels
{
    public class CriteriaVM
    {
        public int Id { get; set; }
        public string Description { get; set; }

        [Display(Name = "Criteria")]
        public string Name { get; set; }
    }
}