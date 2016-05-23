using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WeBind.Models
{
    public class StudentBlogViewModel
    {
        public int StudentBlogID { get; set; }
        [AllowHtml]
        public string Blog { get; set; }
        public string BlogTitle { get; set; }
        public long StudentID { get; set; }
        public Nullable<bool> IsApproved { get; set; }
        public bool IsWrittenByMe { get; set; }
        public System.DateTime CreationDate { get; set; }
        public System.DateTime UpdationDate { get; set; }

        public StudentViewModel StudentProfile { get; set; }
    }
}