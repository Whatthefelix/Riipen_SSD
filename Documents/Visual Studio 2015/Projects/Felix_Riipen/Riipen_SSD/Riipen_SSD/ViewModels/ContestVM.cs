using Riipen_SSD.AdminViewModels;
using System;
using System.Collections.Generic;


namespace Riipen_SSD.ViewModels
{
    public class ContestVM
    {
        public String ContestName { get; set; }
        public DateTime Date { get; set; }
        public String Location { get; set; }
        public List<CriteriaVM> Criteria { get; set; }
        public List<JudgeVM> Judges { get; set; }
        }
    }
