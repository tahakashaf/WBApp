using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeBind.Models
{
    public class CampusRequestViewModel
    {
        public string RequestMessage { get; set; }
        public long CampusID { get; set; }
        public System.DateTime RequestedOn { get; set; }
        public bool IsSubmitted { get; set; }
        public string CampusName { get; set; }
        public List<WebinarViewModel> UpcomingMCList { get; set; }
    }
}