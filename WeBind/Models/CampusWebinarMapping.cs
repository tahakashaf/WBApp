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
    
    public partial class CampusWebinarMapping
    {
        public CampusWebinarMapping()
        {
            this.WebinarFeedBacks = new HashSet<WebinarFeedBack>();
        }
    
        public long CampusWebinarID { get; set; }
        public long WebinarID { get; set; }
        public long CampusID { get; set; }
        public Nullable<bool> IsApproved { get; set; }
    
        public virtual ICollection<WebinarFeedBack> WebinarFeedBacks { get; set; }
        public virtual Webinar Webinar { get; set; }
        public virtual CampusProfile CampusProfile { get; set; }
    }
}
