using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeBind.Models
{
    public class CertificateQueViewModel
    {
        public long QuestionID { get; set; }
        public Nullable<long> WebinarID { get; set; }
        public string Question { get; set; }
        public WebinarViewModel Webinar { get; set; }
        public IEnumerable<CertificateAnswersViewModel> WebinarAnswers { get; set; }
    }
}