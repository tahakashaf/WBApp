using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeBind.Models
{
    public class BrandViewModel
    {
        public long BrandID { get; set; }
        public string BrandName { get; set; }
        public string EmailID { get; set; }
        public string Password { get; set; }
        public string ContactNo { get; set; }
        public long BrandLogoID { get; set; }
        public string BrandLink { get; set; }
        public string YoutubeURL { get; set; }
        public string BrandContent { get; set; }
        public string ProfilePicPath { get; set; }
        public ProfilePic BrandLogo { get; set; }
        public IEnumerable<WebinarViewModel> Webinars { get; set; }
        public IEnumerable<BrandBannerImage> BrandBannerImages { get; set; }
        public IEnumerable<StudentBlogViewModel> StudentBlogs { get; set; }
        public BrandTableReportViewModel BrandReport { get; set; }
    }
    public class BrandTableReportViewModel
    {
        public long TotalSessions { get; set; }
        public long Mailers { get; set; }
        public long StudentsRegistered { get; set; }
        public long NewsLetterClicks { get; set; }
        public long CollegesAttended { get; set; }
        public long CertificatesDownloaded { get; set; }
        public long CouponsRedeeemed { get; set; }
        public long Blogs { get; set; }
    }
}