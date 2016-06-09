using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeBind.Models
{
    public class CampusViewModel
    {
        public long CampusID { get; set; }
        public string CampusName { get; set; }
        public string DepartmentName { get; set; }
        public string Street { get; set; }
        public string Area { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public string ContactNo { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
        public System.DateTime RegisteredOn { get; set; }
        public string EmailID { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<long> ProfilePicID { get; set; }
        public Nullable<bool> IsConfirmed { get; set; }
        public string ProfilePicPath { get; set; }
        public virtual ICollection<WebinarViewModel> Webinars { get; set; }
        public string ProfessorName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public long Participants { get; set; }
    }
}
