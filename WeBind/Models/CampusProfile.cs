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
    
    public partial class CampusProfile
    {
        public CampusProfile()
        {
            this.CampusDepartments = new HashSet<CampusDepartment>();
            this.CampusExpertRequests = new HashSet<CampusExpertRequest>();
            this.CampusRequests = new HashSet<CampusRequest>();
            this.CampusWebinarMappings = new HashSet<CampusWebinarMapping>();
            this.Messages = new HashSet<Message>();
        }
    
        public long CampusID { get; set; }
        public string CampusName { get; set; }
        public string DepartmentName { get; set; }
        public string Street { get; set; }
        public string Area { get; set; }
        public string District { get; set; }
        public string State { get; set; }
        public string ContactNo { get; set; }
        public string LoginID { get; set; }
        public string Password { get; set; }
        public System.DateTime RegisteredOn { get; set; }
        public string EmailID { get; set; }
        public Nullable<System.DateTime> LastUpdated { get; set; }
        public Nullable<long> ProfilePicID { get; set; }
        public Nullable<bool> IsConfirmed { get; set; }
        public bool IsPartnerCollege { get; set; }
        public string ProfessorName { get; set; }
    
        public virtual ICollection<CampusDepartment> CampusDepartments { get; set; }
        public virtual ICollection<CampusExpertRequest> CampusExpertRequests { get; set; }
        public virtual ProfilePic ProfilePic { get; set; }
        public virtual ICollection<CampusRequest> CampusRequests { get; set; }
        public virtual ICollection<CampusWebinarMapping> CampusWebinarMappings { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
