//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WeBind.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class RecommandAFriend
    {
        public long RecommandationID { get; set; }
        public long ExpertID { get; set; }
        public string FriendEmailIDs { get; set; }
        public string Message { get; set; }
        public System.DateTime RecommandedOn { get; set; }
    
        public virtual ExpertProfile ExpertProfile { get; set; }
    }
}
