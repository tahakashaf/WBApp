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
                long CampusID = ViewBag.UserData.CampusID;
                return View();
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

        [HttpGet]
        public JsonResult GetMapData()
        {
            long BrandID = ViewBag.UserData.BrandID;
            List<Webinar> webinars = _Context.Webinars.Where(x => x.BrandID == BrandID).ToList();
            List<WebinarViewModel> webinarVms = new List<WebinarViewModel>();
            foreach (Webinar webinar in webinars)
            {
                WebinarViewModel w = new WebinarViewModel();
                w.WebinarID = webinar.WebinarID;
                w.WebinarName = webinar.WebinarName;
                w.WebinarPicPath = webinar.ProfilePic.ProfilePicPath;
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
            var latitude = point.Latitude;
            return latitude;
        }
        public double GetLongitude(string address)
        {
            var locationService = new GoogleLocationService();
            var point = locationService.GetLatLongFromAddress(address);
            var Longitude = point.Longitude;
            return Longitude;
        }
    }
}