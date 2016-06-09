using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBind.Helper;
using WeBind.Models;
using GoogleMaps.LocationServices;

namespace WeBind.Controllers
{
    [Authorize(Roles = "Brand")]
    public class BrandController : BaseController
    {
        WeBindDemoEntities _Context = new WeBindDemoEntities();
        DateTimeHelper dateTimeHelper = null;
        EmailHelper emailHelper = null;
        public static string[] States = new string[] { "Andaman and Nicobar Islands", "Andhra Pradesh", "Arunachal Pradesh", "Assam", "Bihar", "Chandigarh", "Chhattisgarh", "Dadra and Nagar Haveli", "Daman and Diu", "Delhi", "Goa", "Gujarat", "Haryana", "Himachal Pradesh", "Jammu and Kashmir", "Jharkhand", "Karnataka", "Kerala", "Lakshadweep", "Madhya Pradesh", "Maharashtra", "Manipur", "Meghalaya", "Mizoram", "Nagaland", "Orissa", "Pondicherry", "Punjab", "Rajasthan", "Sikkim", "Tamil Nadu", "Tripura", "Uttaranchal", "Uttar Pradesh", "West Bengal" };

        // GET: Brand
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BrandDashBoard()
        {
            SelectList list = new SelectList(States);
            ViewBag.statesList = list;
            try
            {
                long BrandID = ViewBag.UserData.BrandID;
                BrandProfile Brand = _Context.BrandProfiles.Find(BrandID);
                List<long> AttendedClgIds = new List<long>();
                foreach (Webinar webinar in Brand.Webinars)
                {
                    AttendedClgIds.AddRange(webinar.CampusWebinarMappings.Where(x => x.IsApproved == true).Select(x => x.CampusID));
                }
                BrandViewModel BrandVM = new BrandViewModel();
                BrandVM.BrandBannerImages = Brand.BrandBannerImages;
                BrandVM.BrandContent = Brand.BrandContent;
                BrandVM.BrandID = Brand.BrandID;
                BrandVM.BrandLink = Brand.BrandLink;
                BrandVM.ProfilePicPath = Brand.ProfilePic.ProfilePicPath;
                BrandVM.BrandLogoID = Brand.BrandLogoID;
                BrandVM.BrandName = Brand.BrandName;
                BrandVM.ContactNo = Brand.ContactNo;
                BrandVM.EmailID = Brand.EmailID;
                BrandVM.YoutubeURL = Brand.YoutubeURL;
                BrandVM.Webinars = Brand.Webinars.Select(x => new WebinarViewModel()
                {
                    WebinarID = x.WebinarID,
                    WebinarName = x.WebinarName,
                    WebinarPicPath = x.ProfilePic.ProfilePicPath
                }).ToList();
                BrandVM.BrandReport = new BrandTableReportViewModel()
                {
                    TotalSessions = Brand.Webinars.Count(),
                    Mailers = 100,
                    StudentsRegistered = _Context.StudentProfiles.Count(),
                    NewsLetterClicks = 200,
                    CollegesAttended = AttendedClgIds.Distinct().Count(),
                    CertificatesDownloaded = 300,
                    CouponsRedeeemed = 2000,
                    Blogs = Brand.StudentBlogs.Count()
                };
                return View(BrandVM);
            }
            catch (Exception ex)
            {
                try
                {
                    EmailViewModel mail = new EmailViewModel();
                    mail.MailToList = new List<string>();
                    mail.MailBCC = new List<string>();
                    mail.MailSubject = Constants.ExceptionErrorMailSubject;
                    mail.MailBody = string.Format(Constants.ExceptionErrorMailBody, ex.Message, ex.StackTrace);
                    emailHelper.SendMail(mail);
                }
                catch (Exception)
                {
                }
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult BrandDashBoard(long WebinarID = 0, string State = null)
        {
            SelectList list = new SelectList(States);
            ViewBag.statesList = list;

            if (WebinarID != 0)
            {
                ViewBag.Webinar = _Context.Webinars.Find(WebinarID).WebinarName;
            }

            if (State != null)
            {
                ViewBag.State = State;
            }
            int webinarCount = 0;
            long BrandID = ViewBag.UserData.BrandID;
            BrandProfile Brand = _Context.BrandProfiles.Find(BrandID);
            List<long> AttendedClgIds = new List<long>();
            foreach (Webinar webinar in Brand.Webinars.Where(x => WebinarID == 0 || (WebinarID != 0 &&  x.WebinarID == WebinarID)))
            {
                webinarCount++;
                AttendedClgIds.AddRange(webinar.CampusWebinarMappings
                    .Where(x => x.IsApproved == true)
                    .Where(x => string.IsNullOrEmpty(State) || (!string.IsNullOrEmpty(State) && x.CampusProfile.State.ToLower().Equals(State.ToLower())))
                    .Select(x => x.CampusID));
            } 
            BrandViewModel BrandVM = new BrandViewModel();
            BrandVM.BrandBannerImages = Brand.BrandBannerImages;
            BrandVM.BrandContent = Brand.BrandContent;
            BrandVM.BrandID = Brand.BrandID;
            BrandVM.BrandLink = Brand.BrandLink;
            BrandVM.ProfilePicPath = Brand.ProfilePic.ProfilePicPath;
            BrandVM.BrandLogoID = Brand.BrandLogoID;
            BrandVM.BrandName = Brand.BrandName;
            BrandVM.ContactNo = Brand.ContactNo;
            BrandVM.EmailID = Brand.EmailID;
            BrandVM.YoutubeURL = Brand.YoutubeURL;
            BrandVM.Webinars = Brand.Webinars.Select(x => new WebinarViewModel()
            {
                WebinarID = x.WebinarID,
                WebinarName = x.WebinarName,
                WebinarPicPath = x.ProfilePic.ProfilePicPath
            }).ToList();
            BrandVM.BrandReport = new BrandTableReportViewModel()
            {
                TotalSessions = webinarCount,
                Mailers = 100,
                StudentsRegistered = _Context.StudentProfiles.Count(),
                NewsLetterClicks = 200,
                CollegesAttended = AttendedClgIds.Distinct().Count(),
                CertificatesDownloaded = 300,
                CouponsRedeeemed = 2000,
                Blogs = Brand.StudentBlogs.Count()
            };
                
            return View(BrandVM);
        }

        [HttpGet]
        public JsonResult GetMapData(long WebinarID = 0, string State = null)
        {
            long BrandID = ViewBag.UserData.BrandID;
            List<Webinar> webinars = _Context.Webinars.Where(x => x.BrandID == BrandID).ToList();
            if (WebinarID > 0)
            {
                webinars = webinars.Where(x => x.WebinarID == WebinarID).ToList();
            }
            List<WebinarViewModel> webinarVms = new List<WebinarViewModel>();
            foreach (Webinar webinar in webinars)
            {
                WebinarViewModel w = new WebinarViewModel();
                w.WebinarID = webinar.WebinarID;
                w.WebinarName = webinar.WebinarName;
                w.WebinarPicPath = webinar.ProfilePic.ProfilePicPath;
                w.ParticipantCount = webinar.CampusWebinarMappings.Sum(y => y.ParticipantCount);
                w.Campuses = webinar.CampusWebinarMappings
                    .Where(x => x.IsApproved == true)
                    .Where(x => (x.CampusProfile.State.ToLower().Equals(State.ToLower()) && !string.IsNullOrEmpty(State)) || string.IsNullOrEmpty(State))
                    .Select(z => new CampusViewModel()
                {
                    CampusID = z.CampusID,
                    CampusName = z.CampusProfile.CampusName,
                    DepartmentName = z.CampusProfile.DepartmentName,
                    District = z.CampusProfile.District,
                    State = z.CampusProfile.State,
                    Street = z.CampusProfile.Street,
                    Area = z.CampusProfile.Area,
                    Latitude = GetLatitude(z.CampusProfile.CampusName + ", " + z.CampusProfile.Street + ", " + z.CampusProfile.District + ", " + z.CampusProfile.State),
                    Longitude = GetLongitude(z.CampusProfile.CampusName + ", " + z.CampusProfile.Street + ", " + z.CampusProfile.District + ", " + z.CampusProfile.State),
                    ProfilePicPath = z.CampusProfile.ProfilePic.ProfilePicPath,
                    Participants = z.ParticipantCount
                }).ToList();
                webinarVms.Add(w);
            }

            return Json(webinarVms, JsonRequestBehavior.AllowGet);
        }


        public double GetLatitude(string address)
        {
            var locationService = new GoogleLocationService();
            var point = locationService.GetLatLongFromAddress(address);
            var latitude = 0.0;
            if (point != null)
            {
                latitude = point.Latitude;
            }
            return latitude;
        }
        public double GetLongitude(string address)
        {
            var locationService = new GoogleLocationService();
            var point = locationService.GetLatLongFromAddress(address);
            var Longitude = 0.0;
            if (point != null)
            {
                Longitude = point.Longitude;
            }
            return Longitude;
        }
    }
}