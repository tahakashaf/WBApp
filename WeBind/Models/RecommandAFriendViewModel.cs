using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeBind.Models
{
    public class RecommandAFriendViewModel
    {
        public long RecommandationID { get; set; }
        public long ExpertID { get; set; }
        public string FriendEmailIDs { get; set; }
        public string Message { get; set; }
        public System.DateTime RecommandedOn { get; set; }
    }
}