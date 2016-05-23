using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeBind.Models
{
    public class StudentViewModel
    {
        public long StudentID { get; set; }
        public string StudentName { get; set; }
        public string EmailID { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
        public string CampusName { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<long> Year { get; set; }
        public string ContactNumber { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<long> ProfilePicID { get; set; }
        public System.DateTime RegisteredOn { get; set; }
        public Nullable<bool> IsConfirmed { get; set; }
        public string ProfilePicPath { get; set; }
        public virtual ICollection<WebinarViewModel> Webinars { get; set; }
    }
}
