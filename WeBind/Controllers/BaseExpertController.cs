using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBind.Models;

namespace WeBind.Controllers
{
    public class BaseController : Controller
    {
        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            string userEmail = requestContext.HttpContext.User.Identity.Name;
            if (!string.IsNullOrEmpty(userEmail))
            {
                ApplicationBaseModel data = null;
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    ExpertProfile expert = _Context.ExpertProfiles.Where(x => x.EmailID == userEmail).FirstOrDefault();
                    if (expert != null)
                    {
                        data = new ApplicationBaseModel()
                        {
                            ExpertID = expert.ExpertID,
                            FirstName = expert.FirstName.Split(' ')[0],
                            LastName = expert.LastName,
                            ProfilePicPath = expert.ProfilePic.ProfilePicPath,
                            ProfileCompleted = expert.ProfileCompleted,
                            RoleName = Roles.Expert.ToString()
                        };
                    }
                    CampusProfile campus = _Context.CampusProfiles.Where(x => x.EmailID == userEmail).FirstOrDefault();
                    if (campus != null)
                    {
                        data = new ApplicationBaseModel()
                        {
                            CampusID = campus.CampusID,
                            CampusName = campus.CampusName,
                            ProfilePicPath = campus.ProfilePic.ProfilePicPath,
                            RoleName = Roles.Educator.ToString()
                        };
                    }
                    StudentProfile student = _Context.StudentProfiles.Where(x => x.EmailID == userEmail).FirstOrDefault();
                    if (student != null)
                    {
                        data = new ApplicationBaseModel()
                        {
                            StudentID = student.StudentID,
                            StudentName = student.StudentName,
                            ProfilePicPath = student.ProfilePic.ProfilePicPath,
                            RoleName = Roles.Student.ToString()
                        };
                    }
                    BrandProfile brand = _Context.BrandProfiles.Where(x => x.EmailID == userEmail).FirstOrDefault();
                    if (brand != null)
                    {
                        data = new ApplicationBaseModel()
                        {
                            BrandID = brand.BrandID,
                            StudentName = brand.BrandName,
                            ProfilePicPath = brand.ProfilePic.ProfilePicPath,
                            RoleName = Roles.Brand.ToString()
                        };
                    }
                }
                ViewBag.UserData = data;
            }
           
        }
    }
}