using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeBind.Models
{
    public class MessageViewModel
    {
        public MessageContentViewModel PostMessage { get; set; }
        public virtual ICollection<CampusMessagesViewModel> Campuses { get; set; }
        public virtual ICollection<ExpertMessagesViewModel> Experts { get; set; }
        public virtual ICollection<MessageContentViewModel> Messages { get; set; }

    }

    public class MessageContentViewModel
    {
        public long MessageID { get; set; }
        public string Message { get; set; }
        public string MessageFrom { get; set; }
        public string CampusName { get; set; }
        public string DepartmentName { get; set; }
        public long CampusID { get; set; }
        public string ExpertFirstName { get; set; }
        public string ExpertLastName { get; set; }
        public long ExpertID { get; set; }
        public DateTime MessageDateTime { get; set; }
        public string MessageAgoDateTime { get; set; }
        public string ProfilePicPath { get; set; }
    }

    public class CampusMessagesViewModel
    {
        public long CampusID { get; set; }
        public string CampusName { get; set; }
        public string DepartmentName { get; set; }
        public string ProfilePicPath { get; set; }
        public string LastMessage { get; set; }
        public string MessageAgoDateTime { get; set; }
        public DateTime MessageDateTime { get; set; }
    }

    public class ExpertMessagesViewModel
    {
        public long ExpertID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePicPath { get; set; }
        public string LastMessage { get; set; }
        public string MessageAgoDateTime { get; set; }
        public DateTime MessageDateTime { get; set; }
    }
}