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
    
    public partial class Message
    {
        public long MessageID { get; set; }
        public string Message1 { get; set; }
        public long ExpertID { get; set; }
        public long CampusID { get; set; }
        public System.DateTime MessageDateTime { get; set; }
        public bool IsFromExpert { get; set; }
        public Nullable<bool> IsAdminApproved { get; set; }
    
        public virtual CampusProfile CampusProfile { get; set; }
        public virtual ExpertProfile ExpertProfile { get; set; }
    }
}
