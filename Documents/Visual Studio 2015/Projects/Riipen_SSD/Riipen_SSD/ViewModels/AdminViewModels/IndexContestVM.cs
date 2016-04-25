using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Riipen_SSD.AdminViewModels
{
    public class IndexContestVM
    {
        public string Name { get; set; }
        public string StartTime { get; set; }
        public string Location { get; set; }
        public Boolean Published { get; set; }
        public int ContestID { get; set; }
    }
}