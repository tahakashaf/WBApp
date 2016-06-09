using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBind.Helper;
using WeBind.Models;

namespace WeBind.Controllers
{
    public class AdminController : Controller
    {
        DateTimeHelper dateTimeHelper = null;
        EmailHelper emailHelper = null;
        public AdminController()
        {
            dateTimeHelper = new DateTimeHelper();
            emailHelper = new EmailHelper();
        }

        #region Admin Login

        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string name, string password)
        {
            if ("admin".Equals(name) && "webind".Equals(password))
            {
                Session["user"] = "Admin";
                return RedirectToAction("Home");
            }
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }

        #endregion

        #region BackOffice Content

        public ActionResult Home()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        public ActionResult Story()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                using (var db = new WeBindDemoEntities())
                {
                    ConfigurationData story = db.ConfigurationDatas.Where(w => w.Key == "Story").Select(s => s).FirstOrDefault();
                    return View("StoryContent", story);
                }
            }
        }

        [HttpPost]
        public ActionResult Story(ConfigurationData data)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                using (var _Context = new WeBindDemoEntities())
                {
                    ConfigurationData story = _Context.ConfigurationDatas.Where(w => w.Key == "Story").Select(s => s).FirstOrDefault();
                    story.Value = data.Value;
                    story.Default_Value = data.Default_Value;
                    _Context.Entry(story).State = System.Data.Entity.EntityState.Modified;
                    _Context.SaveChanges();
                    TempData["AlertMessage"] = "Successfully Submitted";
                }
                return View("Home");
            }
        }

        public ActionResult Concept()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                using (var db = new WeBindDemoEntities())
                {
                    ConfigurationData concept = db.ConfigurationDatas.Where(w => w.Key == "Concept").Select(s => s).FirstOrDefault();
                    return View("ConceptContent", concept);
                }
            }
        }

        [HttpPost]
        public ActionResult Concept(ConfigurationData data)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                using (var _Context = new WeBindDemoEntities())
                {
                    ConfigurationData concept = _Context.ConfigurationDatas.Where(w => w.Key == "Concept").Select(s => s).FirstOrDefault();
                    concept.Value = data.Value;
                    concept.Default_Value = data.Default_Value;
                    _Context.Entry(concept).State = System.Data.Entity.EntityState.Modified;
                    _Context.SaveChanges();
                    TempData["AlertMessage"] = "Successfully Submitted";
                }
                return View("Home");
            }
        }

        public ActionResult BeingMaster()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                using (var db = new WeBindDemoEntities())
                {
                    ConfigurationData beingMaster = db.ConfigurationDatas.Where(w => w.Key == "BeingMaster").Select(s => s).FirstOrDefault();
                    return View("BeingMasterContent", beingMaster);
                }
            }
        }

        [HttpPost]
        public ActionResult BeingMaster(ConfigurationData data)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                using (var _Context = new WeBindDemoEntities())
                {
                    ConfigurationData beingMaster = _Context.ConfigurationDatas.Where(w => w.Key == "BeingMaster").Select(s => s).FirstOrDefault();
                    beingMaster.Value = data.Value;
                    beingMaster.Default_Value = data.Default_Value;
                    _Context.Entry(beingMaster).State = System.Data.Entity.EntityState.Modified;
                    _Context.SaveChanges();
                    TempData["AlertMessage"] = "Successfully Submitted";
                }
                return View("Home");
            }
        }

        public ActionResult Team()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                using (var db = new WeBindDemoEntities())
                {
                    ConfigurationData team = db.ConfigurationDatas.Where(w => w.Key == "Team").Select(s => s).FirstOrDefault();
                    return View("TeamContent", team);
                }
            }
        }

        [HttpPost]
        public ActionResult Team(ConfigurationData data)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                using (var _Context = new WeBindDemoEntities())
                {
                    ConfigurationData team = _Context.ConfigurationDatas.Where(w => w.Key == "Team").Select(s => s).FirstOrDefault();
                    team.Value = data.Value;
                    team.Default_Value = data.Default_Value;
                    _Context.Entry(team).State = System.Data.Entity.EntityState.Modified;
                    _Context.SaveChanges();
                    TempData["AlertMessage"] = "Successfully Submitted";
                }
                return View("Home");
            }
        }
        #endregion

        #region WebinarFeedback
        public ActionResult WebinarFeedback()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                List<WebinarFeedBackViewModel> feedbackList = new List<WebinarFeedBackViewModel>();
                using (var db = new WeBindDemoEntities())
                {
                    feedbackList = db.WebinarFeedBacks.Where(w => w.IsApproved == null).Select(s => new WebinarFeedBackViewModel()
                    {
                        FeedbackID = s.FeedbackID,
                        FeedBack = s.FeedBack,
                        CampusName = s.CampusWebinarMapping.CampusProfile.CampusName,
                        DepartmentName = s.CampusWebinarMapping.CampusProfile.DepartmentName,
                        WebinarName = s.CampusWebinarMapping.Webinar.WebinarName,
                        ExpertFirstName = s.CampusWebinarMapping.Webinar.ExpertProfile.FirstName,
                        ExpertLastName = s.CampusWebinarMapping.Webinar.ExpertProfile.LastName
                    }).ToList();
                }
                return View(feedbackList);
            }
        }

        [HttpPost, ActionName("ApproveFeedBack")]
        public JsonResult ApproveFeedBack(int id)
        {
            bool result;
            try
            {
                using (var db = new WeBindDemoEntities())
                {
                    WebinarFeedBack webinarFeeback = db.WebinarFeedBacks.Where(w => w.FeedbackID == id).FirstOrDefault();
                    webinarFeeback.IsApproved = true;
                    db.Entry(webinarFeeback).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("RejectFeedBack")]
        public JsonResult RejectFeedBack(int id)
        {
            bool result;
            try
            {
                using (var db = new WeBindDemoEntities())
                {
                    WebinarFeedBack webinarFeeback = db.WebinarFeedBacks.Where(w => w.FeedbackID == id).FirstOrDefault();
                    webinarFeeback.IsApproved = false;
                    db.Entry(webinarFeeback).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Messages

        public ActionResult Messages()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                List<MessageContentViewModel> messageList = new List<MessageContentViewModel>();

                using (var db = new WeBindDemoEntities())
                {
                    messageList = db.Messages.Where(w => w.IsFromExpert == false && w.IsAdminApproved == null).Select(s => new MessageContentViewModel()
                    {
                        MessageID = s.MessageID,
                        Message = s.Message1,
                        CampusName = s.CampusProfile.CampusName,
                        DepartmentName = s.CampusProfile.DepartmentName,
                        ExpertFirstName = s.ExpertProfile.FirstName,
                        ExpertLastName = s.ExpertProfile.LastName
                    }).ToList();
                }
                return View(messageList);
            }
        }

        [HttpPost, ActionName("ApproveMessage")]
        public JsonResult ApproveMessage(int id)
        {
            bool result;
            try
            {
                using (var db = new WeBindDemoEntities())
                {
                    Message message = db.Messages.Where(w => w.MessageID == id).FirstOrDefault();
                    message.IsAdminApproved = true;
                    db.Entry(message).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ActionName("RejectMessage")]
        public JsonResult RejectMessage(int id)
        {
            bool result;
            try
            {
                using (var db = new WeBindDemoEntities())
                {
                    Message message = db.Messages.Where(w => w.MessageID == id).FirstOrDefault();
                    message.IsAdminApproved = false;
                    db.Entry(message).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region EliteMasters
        public ActionResult EliteMaster()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                List<ExpertProfileViewModel> expertsList = new List<ExpertProfileViewModel>();
                using (var db = new WeBindDemoEntities())
                {
                    expertsList = db.ExpertProfiles.Select(s => new ExpertProfileViewModel
                    {
                        ExpertID = s.ExpertID,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Company = s.Company,

                        EmailID = s.EmailID,
                        AboutMe = s.AboutMe,
                        Position = s.Position,
                        ExperienceYear = s.ExperienceYear == null ? 0 : (int)s.ExperienceYear,
                        ExperienceMonth = s.ExperienceMonth == null ? 0 : (int)s.ExperienceMonth,
                        ProfilePicPath = s.ProfilePic.ProfilePicPath,
                        LinkedInProfileLink = s.LinkedInProfileLink,
                        TweeterProfileLink = s.TweeterProfileLink,

                        RegisteredOn = s.RegisteredOn,
                        IsEliteMaster = s.IsEliteMaster
                    }).
                        ToList();
                }
                return View(expertsList);
            }
        }

        [HttpPost, ActionName("SetEliteMaster")]
        public JsonResult SetEliteMaster(int id)
        {
            bool result = false;
            try
            {
                using (var db = new WeBindDemoEntities())
                {
                    ExpertProfile expertProfile = db.ExpertProfiles.Where(w => w.ExpertID == id).FirstOrDefault();
                    expertProfile.IsEliteMaster = true;
                    db.Entry(expertProfile).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }

            }
            catch (Exception ex)
            {
            }
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost, ActionName("UnSetEliteMaster")]
        public JsonResult UnSetEliteMaster(int id)
        {
            bool result = false;
            try
            {
                using (var db = new WeBindDemoEntities())
                {
                    ExpertProfile expertProfile = db.ExpertProfiles.Where(w => w.ExpertID == id).FirstOrDefault();
                    expertProfile.IsEliteMaster = false;
                    db.Entry(expertProfile).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }

            }
            catch (Exception ex)
            {
            }
            return Json(new { result }, JsonRequestBehavior.AllowGet);

        }
        #endregion


        #region Attending Campus Request

        public ActionResult CampusRequestWebinar()
        {


            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                List<CampusWebinarRequestViewModel> list = new List<CampusWebinarRequestViewModel>();
                using (var db = new WeBindDemoEntities())
                {
                    list = db.CampusWebinarMappings.Where(w => w.IsApproved == null).Select(
                        s => new CampusWebinarRequestViewModel
                        {
                            CampusWebinarId = s.CampusWebinarID,
                            Campus = s.CampusProfile.CampusName,
                            Webinar = s.Webinar.WebinarName


                        }
                        ).ToList();

                }
                return View(list);
            }
        }


        [HttpPost, ActionName("Approve")]
        public JsonResult Approve(int id)
        {
            bool result;
            try
            {
                using (var db = new WeBindDemoEntities())
                {
                    CampusWebinarMapping message = db.CampusWebinarMappings.Where(w => w.CampusWebinarID == id).FirstOrDefault();
                    message.IsApproved = true;
                    db.Entry(message).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost, ActionName("Reject")]
        public JsonResult Reject(int id)
        {
            bool result;
            try
            {
                using (var db = new WeBindDemoEntities())
                {
                    CampusWebinarMapping message = db.CampusWebinarMappings.Where(w => w.CampusWebinarID == id).FirstOrDefault();
                    message.IsApproved = false;
                    db.Entry(message).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }


        #endregion


        #region PartnerCampus
        public ActionResult PartnerColleges()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                List<CampusProfile> campusProfileList = new List<CampusProfile>();
                using (var db = new WeBindDemoEntities())
                {

                    campusProfileList = db.CampusProfiles.ToList().
                        Select(s => new CampusProfile
                        {
                            CampusID = s.CampusID,
                            CampusName = s.CampusName,
                            DepartmentName = s.DepartmentName,
                            ContactNo = s.ContactNo,
                            EmailID = s.EmailID,
                            Street = s.Street,
                            District = s.District,
                            State = s.State,
                            IsConfirmed = s.IsConfirmed,

                            RegisteredOn = s.RegisteredOn,
                            IsPartnerCollege = s.IsPartnerCollege
                        }).
                        ToList();
                }
                return View(campusProfileList);
            }
        }

        [HttpPost, ActionName("SetPartner")]
        public JsonResult SetPartner(int id)
        {
            bool result = false;
            try
            {

                using (var db = new WeBindDemoEntities())
                {
                    CampusProfile campusProfile = db.CampusProfiles.Where(w => w.CampusID == id).FirstOrDefault();
                    campusProfile.IsPartnerCollege = true;
                    db.Entry(campusProfile).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost, ActionName("UnSetPartner")]
        public JsonResult UnSetPartner(int id)
        {
            bool result = false;
            try
            {
                using (var db = new WeBindDemoEntities())
                {
                    CampusProfile campusProfile = db.CampusProfiles.Where(w => w.CampusID == id).FirstOrDefault();
                    campusProfile.IsPartnerCollege = false;
                    db.Entry(campusProfile).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }

            }
            catch (Exception ex)
            {
            }
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region CampusRequest
        public ActionResult CampusRequest()
        {


            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                List<CampusExpertRequestViewModel> campusRequestApprovalList = new List<CampusExpertRequestViewModel>();
                using (var db = new WeBindDemoEntities())
                {

                    campusRequestApprovalList = db.CampusExpertRequests.Where(w => w.IsAdminApproved == null).
                        Select(s => new CampusExpertRequestViewModel
                        {
                            RequestID = s.RequestID,
                            RequestMessage = s.RequestMessage,
                            ScheduleDate = s.ScheduleDate,
                            CampusProfile = new CampusViewModel()
                            {
                                CampusName = s.CampusProfile.CampusName,
                                DepartmentName = s.CampusProfile.DepartmentName
                            },
                            ExpertProfile = new ExpertProfileViewModel()
                            {
                                FirstName = s.ExpertProfile.FirstName,
                                LastName = s.ExpertProfile.LastName
                            },
                            RequestedOn = s.RequestedOn
                        }).
                        ToList();
                }
                return View(campusRequestApprovalList);
            }
        }

        [HttpPost, ActionName("ApproveCampusRequest")]
        public JsonResult ApproveCampusRequest(int id)
        {
            bool result = false;
            try
            {

                using (var db = new WeBindDemoEntities())
                {
                    CampusExpertRequest campusExpertRequest = db.CampusExpertRequests.Where(w => w.RequestID == id).FirstOrDefault();
                    campusExpertRequest.IsAdminApproved = true;
                    db.Entry(campusExpertRequest).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }



        [HttpPost, ActionName("RejectCampusRequest")]
        public JsonResult RejectCampusRequest(long id)
        {
            bool result = false;
            try
            {
                using (var db = new WeBindDemoEntities())
                {
                    CampusExpertRequest campusExpertRequest = db.CampusExpertRequests.Where(w => w.RequestID == id).FirstOrDefault();
                    campusExpertRequest.IsAdminApproved = false;
                    db.Entry(campusExpertRequest).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region  Webinar CRUD
        public ActionResult Webinar()
        {
            WebinarViewModel webinar = new WebinarViewModel();
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                using (var db = new WeBindDemoEntities())
                {
                    List<ExpertProfileViewModel> experts = db.ExpertProfiles.Where(w => w.IsConfirmed == true).Select(x => new ExpertProfileViewModel()
                    {
                        ExpertID = x.ExpertID,
                        FirstName = x.FirstName + " " + x.LastName + " " + " ( " + x.EmailID + " )",
                        LastName = x.LastName,
                        EmailID = x.EmailID
                    }).ToList();
                    webinar.ExpertsList = experts;
                }
            }
            return View(webinar);
        }

        public ActionResult SubmitWebinar(WebinarViewModel webinar, HttpPostedFileBase file)
        {
            try
            {
                if (Session["user"] == null)
                {
                    return RedirectToAction("Login");
                }
                else
                {

                    Webinar webinartobeSaved = null;
                    using (var db = new WeBindDemoEntities())
                    {
                        webinartobeSaved = new Webinar();
                        webinartobeSaved.WebinarName = webinar.WebinarName;
                        webinartobeSaved.WebinarDescription = webinar.WebinarDescription;
                        webinartobeSaved.WebinarSummary = webinar.WebinarSummary;
                        webinartobeSaved.WhatWillYouLearn = webinar.WhatWillYouLearn;
                        webinartobeSaved.FromDateTime = webinar.FromDateTime;
                        webinartobeSaved.TimeDuration = webinar.TimeDuration;
                        webinartobeSaved.YoutubeUrl = webinar.YoutubeUrl;
                        webinartobeSaved.WebinarPicID = 3;
                        webinartobeSaved.statusID = 1;
                        webinartobeSaved.ExpertID = webinar.ExpertID;
                        db.Webinars.Add(webinartobeSaved);


                        db.SaveChanges();

                        //ExpertWebinarMapping ewMap = new ExpertWebinarMapping();
                        //ewMap.ExpertID = webinar.ExpertID;
                        //ewMap.WebinarID = webinartobeSaved.WebinarID;
                        //db.ExpertWebinarMappings.Add(ewMap);
                        //db.SaveChanges();


                        TempData["AlertMessage"] = "Successfully Submitted";


                        if (file != null && webinartobeSaved.WebinarID > 0)
                        {
                            //file = dateTimeHelper.CropImage(file).Save(Path);
                            string serverPath = "~/images/webinarlogo";
                            if (!Directory.Exists(Server.MapPath(serverPath)))
                            {
                                Directory.CreateDirectory(Server.MapPath(serverPath));
                            }
                            string ext = Path.GetExtension(file.FileName);
                            string pic = webinartobeSaved.WebinarID + ext;
                            string path = Path.Combine(Server.MapPath(serverPath), pic);
                            //file.SaveAs(path);
                            dateTimeHelper.CropImage(file).Save(path);
                            using (MemoryStream ms = new MemoryStream())
                            {
                                file.InputStream.CopyTo(ms);
                                byte[] array = ms.GetBuffer();
                            }
                            if (webinartobeSaved.WebinarPicID == 3)
                            {
                                ProfilePic newpic = new ProfilePic() { ProfilePicPath = serverPath + "/" + pic, LastUpdated = DateTime.Now };
                                db.ProfilePics.Add(newpic);
                                db.SaveChanges();
                                webinartobeSaved.WebinarPicID = newpic.ProfilePicID;
                            }
                            else
                            {
                                webinartobeSaved.ProfilePic.ProfilePicPath = serverPath + "/" + pic;
                            }
                            db.Entry(webinartobeSaved).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    return View("Home");
                }
            }
            catch (Exception ex)
            {
                try
                {
                    EmailViewModel mail = new EmailViewModel();
                    mail.MailToList = new List<string>();
                    mail.MailBCC = new List<string>();
                    mail.MailSubject = "WeBind || Error";
                    mail.MailBody = "Hi <br /><br /> Exception : " + ex.Message + "<br />" + ex.StackTrace; ;
                    emailHelper.SendMail(mail);
                }
                catch (Exception)
                {
                }
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult WebinarList()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                List<Webinar> webinars = new List<Webinar>();
                using (var db = new WeBindDemoEntities())
                {
                    webinars = db.Webinars.ToList();
                }
                return View(webinars);
            }
        }


        public ActionResult GetWebinarById(long id)
        {
            WebinarViewModel webinar = new WebinarViewModel();
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                using (var db = new WeBindDemoEntities())
                {
                    Webinar webinarDB = db.Webinars.Find(id);

                    List<ExpertProfileViewModel> experts = db.ExpertProfiles.Where(w => w.IsConfirmed == true).Select(x => new ExpertProfileViewModel()
                    {
                        ExpertID = x.ExpertID,
                        FirstName = x.FirstName + " " + x.LastName + " " + " ( " + x.EmailID + " )",
                        LastName = x.LastName,
                        EmailID = x.EmailID
                    }).ToList();
                    webinar.WebinarID = webinarDB.WebinarID;
                    webinar.WebinarName = webinarDB.WebinarName;
                    webinar.WebinarDescription = webinarDB.WebinarDescription;
                    webinar.WebinarSummary = webinarDB.WebinarSummary;
                    webinar.WhatWillYouLearn = webinarDB.WhatWillYouLearn;
                    webinar.FromDateTime = webinarDB.FromDateTime;
                    webinar.TimeDuration = webinarDB.TimeDuration;
                    webinar.YoutubeUrl = webinarDB.YoutubeUrl;
                    webinar.WebinarPicID = webinarDB.WebinarPicID;
                    webinar.ExpertID = webinarDB.ExpertID;
                    webinar.ExpertsList = experts;
                }
            }
            return View(webinar);

        }


        public ActionResult EditWebinar(long id, WebinarViewModel webinar, HttpPostedFileBase file)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Login");
            }
            else
            {

                Webinar webinartobeSaved = null;
                using (var db = new WeBindDemoEntities())
                {
                    webinartobeSaved = db.Webinars.Find(id);
                    webinartobeSaved.WebinarName = webinar.WebinarName;
                    webinartobeSaved.WebinarDescription = webinar.WebinarDescription;
                    webinartobeSaved.WebinarSummary = webinar.WebinarSummary;
                    webinartobeSaved.WhatWillYouLearn = webinar.WhatWillYouLearn;
                    webinartobeSaved.FromDateTime = webinar.FromDateTime;
                    webinartobeSaved.TimeDuration = webinar.TimeDuration;
                    webinartobeSaved.YoutubeUrl = webinar.YoutubeUrl;
                    webinartobeSaved.ExpertID = webinar.ExpertID;

                    db.Entry(webinartobeSaved).State = System.Data.Entity.EntityState.Modified;

                    db.SaveChanges();

                    //ExpertWebinarMapping ewMap = db.ExpertWebinarMappings.Where(x => x.WebinarID == id).FirstOrDefault();
                    //ewMap.ExpertID = webinar.ExpertID;
                    //db.Entry(ewMap).State = System.Data.Entity.EntityState.Modified;
                    //db.SaveChanges();


                    TempData["AlertMessage"] = "Successfully Submitted";


                    if (file != null && webinartobeSaved.WebinarID > 0)
                    {
                        //file = dateTimeHelper.CropImage(file).Save(Path);
                        string serverPath = "~/images/webinarlogo";
                        if (!Directory.Exists(Server.MapPath(serverPath)))
                        {
                            Directory.CreateDirectory(Server.MapPath(serverPath));
                        }
                        string ext = Path.GetExtension(file.FileName);
                        string pic = webinartobeSaved.WebinarID + ext;
                        string path = Path.Combine(Server.MapPath(serverPath), pic);
                        //file.SaveAs(path);
                        dateTimeHelper.CropImage(file).Save(path);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            file.InputStream.CopyTo(ms);
                            byte[] array = ms.GetBuffer();
                        }
                        if (webinartobeSaved.WebinarPicID == 3)
                        {
                            ProfilePic newpic = new ProfilePic() { ProfilePicPath = serverPath + "/" + pic, LastUpdated = DateTime.Now };
                            db.ProfilePics.Add(newpic);
                            db.SaveChanges();
                            webinartobeSaved.WebinarPicID = newpic.ProfilePicID;
                        }
                        else
                        {
                            webinartobeSaved.ProfilePic.ProfilePicPath = serverPath + "/" + pic;
                        }
                        db.Entry(webinartobeSaved).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return View("Home");
            }
        }

        [HttpPost, ActionName("DeleteWebinar")]
        public JsonResult DeleteWebinar(int id)
        {
            bool result = false;
            try
            {
                using (var db = new WeBindDemoEntities())
                {
                    //List<ExpertWebinarMapping> expertWebinarMapping = db.ExpertWebinarMappings.Where(w => w.WebinarID == id).ToList();
                    //db.ExpertWebinarMappings.RemoveRange(expertWebinarMapping);
                    List<CampusWebinarMapping> campusWebinarMapping = db.CampusWebinarMappings.Where(w => w.WebinarID == id).ToList();
                    db.CampusWebinarMappings.RemoveRange(campusWebinarMapping);
                    List<WebinarTagMapping> webinarTagMapping = db.WebinarTagMappings.Where(w => w.WebinarID == id).ToList();
                    db.WebinarTagMappings.RemoveRange(webinarTagMapping);
                    db.SaveChanges();
                    Webinar webinar = db.Webinars.Find(id);
                    db.Webinars.Remove(webinar);
                    db.SaveChanges();
                    result = true;

                }

            }
            catch (Exception ex)
            {
            }
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }



        #endregion

    }
}