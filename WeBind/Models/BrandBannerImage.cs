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
    
    public partial class BrandBannerImage
    {
        public long BrandImageID { get; set; }
        public string BrandBannerImagePath { get; set; }
        public long BrandID { get; set; }
    
        public virtual BrandProfile BrandProfile { get; set; }
    }
}
