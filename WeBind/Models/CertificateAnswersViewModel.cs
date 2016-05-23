using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeBind.Models
{
    public class CertificateAnswersViewModel
    {
        public long AnswerID { get; set; }
        public Nullable<long> QuestionID { get; set; }
        public string Answer { get; set; }
        public bool IsCorrect { get; set; }
        public bool IsSelected { get; set; }
    }
}