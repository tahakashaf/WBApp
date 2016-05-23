using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeBind.Models
{
    public class ApplicationBaseModel
    {
        public long CampusID { get; set; }
        public string CampusName { get; set; }
        public long ExpertID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long StudentID { get; set; }
        public string StudentName { get; set; }
        public string ProfilePicPath { get; set; }
        public decimal ProfileCompleted { get; set; }
        public string RoleName { get; set; }
    }
}
