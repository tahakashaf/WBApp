using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeBind.Models
{
    public class CampusWebinarRequestViewModel
    {
        public long CampusWebinarId { get; set; }
        public string Campus { get; set; }
        public string Webinar { get; set; }
    }
}