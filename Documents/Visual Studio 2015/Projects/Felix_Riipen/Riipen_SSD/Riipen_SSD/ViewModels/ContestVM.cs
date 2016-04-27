﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.ViewModels
{
    public class ContestVM
    {
        //felix
        public String ContestName { get; set; }

        public DateTime Date { get; set; }

        public String Location { get; set; }

        public List<AdminViewModels.CriteriaVM> Criteria
        {
            get; set;
        }

        public List<AdminViewModels.JudgeVM> Judges
        {
            get; set;
        }

        }
        

    }
