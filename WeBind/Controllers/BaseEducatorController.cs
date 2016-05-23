using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBind.Models;

namespace WeBind.Controllers
{
    [Authorize]
    public class BaseEducatorController : Controller
    {
        // GET: BaseEducator
        public ActionResult Index()
        {
            return View();
        }
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            string userEmail = requestContext.HttpContext.User.Identity.Name;
            if (!string.IsNullOrEmpty(userEmail))
            {
                ApplicationBaseModel data = null;
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    CampusProfile campus = _Context.CampusProfiles.Where(x => x.EmailID == userEmail).FirstOrDefault();
                    data = new ApplicationBaseModel()
                    {
                        CampusID = campus.CampusID,
                        CampusName = campus.CampusName,
                        ProfilePicPath = campus.ProfilePic.ProfilePicPath,
                        RoleName = "Educator"
                    };
                }
                ViewBag.UserData = data;
            }
        }
    }
}