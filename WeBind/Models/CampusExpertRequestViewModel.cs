using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeBind.Models
{
    public class CampusExpertRequestViewModel
    {
        public long RequestID { get; set; }
        public string RequestMessage { get; set; }
        public System.DateTime ScheduleDate { get; set; }
        public long CampusID { get; set; }
        public long RequestedToID { get; set; }
        public System.DateTime RequestedOn { get; set; }
        public Nullable<bool> IsAdminApproved { get; set; }
        public Nullable<bool> IsExpertApproved { get; set; }
        public bool IsRequestSent { get; set; }
        public CampusViewModel CampusProfile { get; set; }
        public ExpertProfileViewModel ExpertProfile { get; set; }
    }
}