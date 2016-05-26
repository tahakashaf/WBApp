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

        // GET: Brand
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BrandDashBoard()
        {
            try
            {
                long BrandID = ViewBag.UserData.BrandID;
                BrandProfile Brand = _Context.BrandProfiles.Find(BrandID);
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

        public ActionResult BrandWebinar(long WebinarID = 0)
        {
            long BrandID = ViewBag.UserData.BrandID;
            BrandProfile Brand = _Context.BrandProfiles.Find(BrandID);
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
            BrandVM.Webinars = Brand.Webinars.Where(x => x.WebinarID == WebinarID).Select(x => new WebinarViewModel()
            {
                WebinarID = x.WebinarID,
                WebinarName = x.WebinarName,
                WebinarPicPath = x.ProfilePic.ProfilePicPath
            }).ToList();
            return View(BrandVM);
        }

        [HttpGet]
        public JsonResult GetMapData(long WebinarID = 0)
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
                w.Participants = webinar.Participants;
                w.Campuses = webinar.CampusWebinarMappings.Where(x => x.IsApproved == true).Select(z => new CampusViewModel()
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
                    ProfilePicPath = z.CampusProfile.ProfilePic.ProfilePicPath
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