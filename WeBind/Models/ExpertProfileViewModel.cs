using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeBind.Models
{
    public class ExpertProfileViewModel
    {
        public long ExpertID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailID { get; set; }
        public string Password { get; set; }
        public string LoginID { get; set; }
        public System.DateTime RegisteredOn { get; set; }
        public string ContactNo { get; set; }
        public string AboutMe { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        public int ExperienceYear { get; set; }
        public int ExperienceMonth { get; set; }
        public List<int> ExperienceYearList { get; set; }
        public List<int> ExperienceMonthList { get; set; }
        public string LinkedInProfileLink { get; set; }
        public string TweeterProfileLink { get; set; }
        public long ProfilePicID { get; set; }
        public string ProfilePicPath { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public string MyTags { get; set; }
        public string ChartValue { get; set; }
        public string AllTags { get; set; }
        public virtual ICollection<string> Tags { get; set; }
        public virtual ICollection<WebinarViewModel> Webinars { get; set; }

        public bool IsEliteMaster { get; set; }
    }
}