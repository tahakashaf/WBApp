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
    
    public partial class CampusDepartment
    {
        public long DepartmentID { get; set; }
        public string DepartmentName { get; set; }
        public long CampusID { get; set; }
    
        public virtual CampusProfile CampusProfile { get; set; }
    }
}
