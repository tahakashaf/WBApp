using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeBind.Models
{
    public class WebinarViewModel
    {
        public long WebinarID { get; set; }
        public string WebinarName { get; set; }
        public string WebinarSummary { get; set; }
        public string WebinarDescription { get; set; }
        public string WhatWillYouLearn { get; set; }
        public System.DateTime FromDateTime { get; set; }
        public System.TimeSpan TimeDuration { get; set; }
        public decimal Participants { get; set; }
        public string YoutubeUrl { get; set; }
        public virtual ICollection<string> Tags { get; set; }
        public long WebinarPicID { get; set; }
        public string WebinarPicPath { get; set; }
        public List<WebinarFeedBackViewModel> FeedBacks { get; set; }
        public long? ExpertID { get; set; }
        public ExpertProfileViewModel Expert { get; set; }
        public long? BrandID { get; set; }
        public BrandViewModel Brand { get; set; }
        public List<ExpertProfileViewModel> ExpertsList { get; set; }
        public List<CampusViewModel> Campuses { get; set; }

        public string FeedBackMsg { get; set; }
        public long CampusWebinarID { get; set; }

        public bool IsWebinarRequested { get; set; }
        public IEnumerable<CertificateQueViewModel> WebinarQuestions { get; set; }
    }

    public class WebinarFeedBackViewModel
    {
        public long FeedbackID { get; set; }
        public string FeedBack { get; set; }
        public System.DateTime SubmittedOn { get; set; }
        public bool IsApproved { get; set; }
        public string WebinarName { get; set; }
        public string CampusName { get; set; }
        public string ExpertFirstName { get; set; }
        public string ExpertLastName { get; set; }
        public string DepartmentName { get; set; }
        public string ProfilePicPath { get; set; }
    }
}
