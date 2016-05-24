using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBind.Helper;
using WeBind.Models;
using System.Data.Entity;

namespace WeBind.Controllers
{
    [Authorize(Roles = "Educator")]
    public class EducatorController : BaseController
    {
        WeBindDemoEntities _Context = new WeBindDemoEntities();
        DateTimeHelper dateTimeHelper = null;
        EmailHelper emailHelper = null;

        public EducatorController()
        {
            dateTimeHelper = new DateTimeHelper();
            emailHelper = new EmailHelper();
        }

        // GET: Educator
        public ActionResult EducatorDashBoard()
        {
            try
            {
                long CampusID = ViewBag.UserData.CampusID;

                List<WebinarViewModel> upcomingMC = _Context.Webinars.OrderBy(x => x.FromDateTime).Where(x => x.FromDateTime > DateTime.Now).Select(x => new WebinarViewModel()
                {
                    WebinarID = x.WebinarID,
                    WebinarName = x.WebinarName,
                    WebinarDescription = x.WebinarDescription,
                    WebinarSummary = x.WebinarSummary,
                    FromDateTime = x.FromDateTime,
                    Tags = x.WebinarTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                    Participants = x.Participants,
                    Campuses = x.CampusWebinarMappings.Select(y => new CampusViewModel()
                    {
                        CampusName = y.CampusProfile.CampusName,
                        ProfilePicPath = y.CampusProfile.ProfilePic.ProfilePicPath
                    }).ToList(),
                    Expert = x.ExpertID == null ? null : new ExpertProfileViewModel()
                    {
                        FirstName = x.ExpertProfile.FirstName,
                        LastName = x.ExpertProfile.LastName,
                        ExpertID = (long)x.ExpertID,
                        ProfilePicPath = x.ExpertProfile.ProfilePic.ProfilePicPath
                    }
                }).ToList();

                CampusViewModel campusDetails = _Context.CampusProfiles.Where(x => x.CampusID == CampusID).Select(x => new CampusViewModel()
                {
                    CampusName = x.CampusName,
                    DepartmentName = x.DepartmentName,
                    Street = x.Street,
                    Area = x.Area,
                    District = x.District,
                    State = x.State,
                    IsConfirmed = x.IsConfirmed,
                    LastUpdated = x.LastUpdated,
                    EmailID = x.EmailID,
                    CampusID = x.CampusID,
                    ContactNo = x.ContactNo,
                    LoginID = x.LoginID,
                    Password = x.Password,
                    ProfilePicID = x.ProfilePicID == null ? 5 : (long)x.ProfilePicID,
                    ProfilePicPath = x.ProfilePic.ProfilePicPath,
                    RegisteredOn = x.RegisteredOn
                }).FirstOrDefault();

                campusDetails.Webinars = upcomingMC;

                return View(campusDetails);
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

        [AllowAnonymous]
        public ActionResult EducatorUpcomingMCList()
        {
            try
            {
                long CampusID = 0;
                if (ViewBag.UserData != null)
                {
                    CampusID = ViewBag.UserData.CampusID;
                }

                List<WebinarViewModel> webinarList = _Context.Webinars.OrderBy(x => x.FromDateTime).Where(x => x.FromDateTime > DateTime.Now).Select(x => new WebinarViewModel()
                {
                    WebinarID = x.WebinarID,
                    WebinarName = x.WebinarName,
                    WebinarDescription = x.WebinarDescription,
                    WebinarSummary = x.WebinarSummary,
                    FromDateTime = x.FromDateTime,
                    Tags = x.WebinarTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                    Participants = x.Participants,
                    WebinarPicPath = x.ProfilePic.ProfilePicPath,
                    IsWebinarRequested = x.CampusWebinarMappings.Where(y => y.CampusID == CampusID).FirstOrDefault() == null ? false : true,
                    Campuses = x.CampusWebinarMappings.Select(y => new CampusViewModel()
                    {
                        CampusName = y.CampusProfile.CampusName,
                        DepartmentName = y.CampusProfile.DepartmentName,
                        ProfilePicPath = y.CampusProfile.ProfilePic.ProfilePicPath
                    }).ToList(),
                    Expert = x.ExpertID == null ? null : new ExpertProfileViewModel()
                    {
                        FirstName = x.ExpertProfile.FirstName,
                        LastName = x.ExpertProfile.LastName,
                        ExpertID = (long)x.ExpertID,
                        ProfilePicPath = x.ExpertProfile.ProfilePic.ProfilePicPath
                    },
                    Brand = x.BrandID == null ? null : new BrandViewModel()
                    {
                        BrandID = x.BrandProfile.BrandID,
                        BrandLink = x.BrandProfile.BrandLink,
                        ProfilePicPath = x.BrandProfile.ProfilePic.ProfilePicPath,
                        YoutubeURL = x.BrandProfile.YoutubeURL,
                        BrandContent = x.BrandProfile.BrandContent,
                        BrandName = x.BrandProfile.BrandName,
                        ContactNo = x.BrandProfile.ContactNo,
                    }
                }).ToList();
                return View(webinarList);
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


        public ActionResult EducatorProfile()
        {
            try
            {
                long CampusID = ViewBag.UserData.CampusID;
                CampusViewModel educatorDetails = _Context.CampusProfiles.Where(x => x.CampusID == CampusID).Select(x => new CampusViewModel()
                {
                    CampusName = x.CampusName,
                    DepartmentName = x.DepartmentName,
                    LastUpdated = x.LastUpdated,
                    EmailID = x.EmailID,
                    CampusID = x.CampusID,
                    ContactNo = x.ContactNo,
                    Street = x.Street,
                    Area = x.Area,
                    District = x.District,
                    State = x.State,
                    LoginID = x.LoginID,
                    Password = x.Password,
                    ProfilePicID = x.ProfilePicID == null ? 0 : (long)x.ProfilePicID,
                    ProfilePicPath = x.ProfilePic.ProfilePicPath,
                    RegisteredOn = x.RegisteredOn,
                }).FirstOrDefault();
                return View(educatorDetails);
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
        public ActionResult EducatorProfile(CampusViewModel educatorDetails, long CampusID, HttpPostedFileBase file)
        {
            try
            {
                // TODO: Add update logic here
                Models.CampusProfile educator = _Context.CampusProfiles.Find(CampusID);

                if (file != null)
                {
                    //file = dateTimeHelper.CropImage(file).Save(Path);
                    string serverPath = "~/images/people/EducatorsPhoto";
                    if (!Directory.Exists(Server.MapPath(serverPath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(serverPath));
                    }
                    string ext = Path.GetExtension(file.FileName);
                    string pic = educator.CampusID + ext;
                    string path = Path.Combine(Server.MapPath(serverPath), pic);
                    //file.SaveAs(path);
                    dateTimeHelper.CropImage(file).Save(path);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                    if (educator.ProfilePicID == 2)
                    {
                        ProfilePic newpic = new ProfilePic() { ProfilePicPath = serverPath + "/" + pic, LastUpdated = DateTime.Now };
                        _Context.ProfilePics.Add(newpic);
                        _Context.SaveChanges();
                        educator.ProfilePicID = newpic.ProfilePicID;
                    }
                    else
                    {
                        educator.ProfilePic.ProfilePicPath = serverPath + "/" + pic;
                    }
                }


                educator.Street = educatorDetails.Street;
                educator.Area = educatorDetails.Area;
                educator.District = educatorDetails.District;
                educator.State = educatorDetails.State;
                educator.DepartmentName = educatorDetails.DepartmentName;
                educator.EmailID = educatorDetails.EmailID;
                educator.CampusID = educatorDetails.CampusID;
                educator.CampusName = educatorDetails.CampusName;
                educator.ContactNo = educatorDetails.ContactNo;
                educator.LastUpdated = DateTime.Now;

                _Context.Entry(educator).State = System.Data.Entity.EntityState.Modified;
                _Context.SaveChanges();

                return RedirectToAction("EducatorProfile", "Educator");
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

        public ActionResult EducatorSessionRequest()
        {
            try
            {
                long CampusID = ViewBag.UserData.CampusID;

                CampusRequestViewModel campusReq = new CampusRequestViewModel();

                List<CampusExpertRequestViewModel> requests = _Context.CampusExpertRequests.Where(x => x.CampusID == CampusID).Select(x => new CampusExpertRequestViewModel()
                {
                    RequestMessage = x.RequestMessage,
                    ScheduleDate = x.ScheduleDate,
                    ExpertProfile = new ExpertProfileViewModel()
                    {
                        ExpertID = x.ExpertProfile.ExpertID,
                        FirstName = x.ExpertProfile.FirstName,
                        LastName = x.ExpertProfile.LastName,
                        ProfilePicPath = x.ExpertProfile.ProfilePic.ProfilePicPath
                    },
                    IsExpertApproved = x.IsExpertApproved
                }).ToList();

                ViewData["Requests"] = requests;

                //List<WebinarViewModel> upcomingMC = _Context.Webinars.OrderBy(x => x.FromDateTime).Where(x => x.FromDateTime > DateTime.Now).Select(x => new WebinarViewModel()
                //{
                //    WebinarID = x.WebinarID,
                //    WebinarName = x.WebinarName,
                //    WebinarSummary = x.WebinarSummary,
                //    FromDateTime = x.FromDateTime,
                //    Tags = x.WebinarTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                //    Experts = x.ExpertWebinarMappings.Select(y => new ExpertProfileViewModel()
                //    {
                //        FirstName = y.ExpertProfile.FirstName,
                //        LastName = y.ExpertProfile.LastName,
                //        ExpertID = y.ExpertID,
                //        ProfilePicPath = y.ExpertProfile.ProfilePic.ProfilePicPath
                //    }).ToList()
                //}).ToList();

                //campusReq.UpcomingMCList = upcomingMC;
                //campusReq.IsSubmitted = false;
                return View(campusReq);
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

        public ActionResult ExpertSearch()
        {
            try
            {
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

        [HttpPost]
        public ActionResult EducatorSessionRequest(CampusRequestViewModel campusReqVM)
        {
            try
            {
                long CampusID = ViewBag.UserData.CampusID;
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    CampusRequest campusReq = new CampusRequest()
                    {
                        RequestMessage = campusReqVM.RequestMessage,
                        RequestedOn = DateTime.Now,
                        CampusID = CampusID
                    };
                    _Context.CampusRequests.Add(campusReq);
                    _Context.SaveChanges();

                    try
                    {
                        CampusRequest camReq = _Context.CampusRequests.Include(x => x.CampusProfile).Where(x => x.expertRequestID == campusReq.expertRequestID).FirstOrDefault();
                        EmailViewModel mail = new EmailViewModel();
                        mail.MailToList = new List<string>();
                        mail.MailBCC = new List<string>();
                        mail.MailToList.Add(Constants.AdminEmailId);
                        mail.MailSubject = "Campus Session Request || " + camReq.CampusProfile.CampusName;
                        mail.MailBody = "Campus Name: " + camReq.CampusProfile.CampusName + " Dept: " + camReq.CampusProfile.DepartmentName
                            + "<br /> Request ID : " + camReq.expertRequestID + "<br /> Request Message: " + camReq.RequestMessage;
                        emailHelper.SendMail(mail);
                    }
                    catch (Exception)
                    {
                    }

                    List<CampusExpertRequestViewModel> requests = _Context.CampusExpertRequests.Where(x => x.CampusID == CampusID).Select(x => new CampusExpertRequestViewModel()
                    {
                        RequestMessage = x.RequestMessage,
                        ScheduleDate = x.ScheduleDate,
                        ExpertProfile = new ExpertProfileViewModel()
                        {
                            ExpertID = x.ExpertProfile.ExpertID,
                            FirstName = x.ExpertProfile.FirstName,
                            LastName = x.ExpertProfile.LastName,
                            ProfilePicPath = x.ExpertProfile.ProfilePic.ProfilePicPath
                        },
                        IsExpertApproved = x.IsExpertApproved
                    }).ToList();

                    ViewData["Requests"] = requests;
                }
                //List<WebinarViewModel> upcomingMC = _Context.Webinars.OrderBy(x => x.FromDateTime).Where(x => x.FromDateTime > DateTime.Now).Select(x => new WebinarViewModel()
                //{
                //    WebinarID = x.WebinarID,
                //    WebinarName = x.WebinarName,
                //    WebinarSummary = x.WebinarSummary,
                //    FromDateTime = x.FromDateTime,
                //    Tags = x.WebinarTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                //    Experts = x.ExpertWebinarMappings.Select(y => new ExpertProfileViewModel()
                //    {
                //        FirstName = y.ExpertProfile.FirstName,
                //        LastName = y.ExpertProfile.LastName,
                //        ExpertID = y.ExpertID,
                //        ProfilePicPath = y.ExpertProfile.ProfilePic.ProfilePicPath
                //    }).ToList()
                //}).ToList();

                campusReqVM = new CampusRequestViewModel();
                campusReqVM.IsSubmitted = true;
                //campusReqVM.UpcomingMCList = upcomingMC;

                return View(campusReqVM);
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

        public ActionResult EducatorMessages(long ExpertID = 0)
        {
            try
            {
                long CampusID = ViewBag.UserData.CampusID;
                List<ExpertMessagesViewModel> expertMsgVM = _Context.Messages.Where(x => x.CampusID == CampusID).GroupBy(x => x.ExpertID).Select(x => new ExpertMessagesViewModel()
                {
                    ExpertID = x.OrderByDescending(y => y.MessageDateTime).Select(y => y.ExpertProfile.ExpertID).FirstOrDefault(),
                    FirstName = x.OrderByDescending(y => y.MessageDateTime).Select(y => y.ExpertProfile.FirstName).FirstOrDefault(),
                    LastName = x.OrderByDescending(y => y.MessageDateTime).Select(y => y.ExpertProfile.LastName).FirstOrDefault(),
                    LastMessage = x.OrderByDescending(y => y.MessageDateTime).Select(y => y.Message1).FirstOrDefault(),
                    MessageDateTime = x.OrderByDescending(y => y.MessageDateTime).Select(y => y.MessageDateTime).FirstOrDefault(),
                    ProfilePicPath = x.OrderByDescending(y => y.MessageDateTime).Select(y => y.ExpertProfile.ProfilePic.ProfilePicPath).FirstOrDefault()
                }).ToList();

                expertMsgVM = expertMsgVM.OrderByDescending(x => x.MessageDateTime).Select(x => new ExpertMessagesViewModel()
                {
                    MessageAgoDateTime = dateTimeHelper.GetAgoTime(x.MessageDateTime),
                    ExpertID = x.ExpertID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    LastMessage = x.LastMessage,
                    MessageDateTime = x.MessageDateTime,
                    ProfilePicPath = x.ProfilePicPath
                }).ToList();

                List<MessageContentViewModel> messageContentVM = new List<MessageContentViewModel>();
                if (expertMsgVM.Count > 0 && ExpertID == 0)
                {
                    ExpertID = expertMsgVM[0].ExpertID;
                }
                messageContentVM = GetExpertMessages(ExpertID, CampusID);

                MessageViewModel msgVM = new MessageViewModel()
                {
                    PostMessage = new MessageContentViewModel()
                    {
                        ExpertID = ExpertID,
                        CampusID = CampusID
                    },
                    Experts = expertMsgVM,
                    Messages = messageContentVM
                };
                return View(msgVM);
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

        private List<MessageContentViewModel> GetExpertMessages(long ExpertID, long CampusID)
        {
            try
            {
                List<MessageContentViewModel> messageContentVM = _Context.Messages.OrderByDescending(x => x.MessageDateTime).Where(x => x.ExpertID == ExpertID && x.CampusID == CampusID).Select(x => new MessageContentViewModel()
                {
                    Message = x.Message1,
                    MessageFrom = x.IsFromExpert ? x.ExpertProfile.FirstName + " " + x.ExpertProfile.LastName : x.CampusProfile.CampusName,
                    ProfilePicPath = x.IsFromExpert ? x.ExpertProfile.ProfilePic.ProfilePicPath : x.CampusProfile.ProfilePic.ProfilePicPath,
                    MessageDateTime = x.MessageDateTime
                }).ToList();

                messageContentVM = messageContentVM.Select(x => new MessageContentViewModel()
                {
                    Message = x.Message,
                    MessageFrom = x.MessageFrom,
                    ProfilePicPath = x.ProfilePicPath,
                    MessageAgoDateTime = dateTimeHelper.GetAgoTime(x.MessageDateTime)
                }).ToList();
                return messageContentVM;
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
                return null;
            }
        }

        [HttpPost]
        public ActionResult PostMessage(MessageViewModel messageVM)
        {
            try
            {

                if (messageVM.PostMessage.Message != null && messageVM.PostMessage.Message.Trim() != "")
                {
                    Message message = new Message()
                    {
                        CampusID = ViewBag.UserData.CampusID,
                        ExpertID = messageVM.PostMessage.ExpertID,
                        IsFromExpert = false,
                        Message1 = messageVM.PostMessage.Message.Trim(),
                        MessageDateTime = DateTime.Now,
                    };
                    _Context.Messages.Add(message);
                    _Context.SaveChanges();

                    try
                    {
                        Message msg = _Context.Messages.Include(x => x.ExpertProfile).Include(x => x.CampusProfile).Where(x => x.MessageID == message.MessageID).FirstOrDefault();
                        EmailViewModel mail = new EmailViewModel();
                        mail.MailToList = new List<string>();
                        mail.MailBCC = new List<string>();
                        mail.MailToList.Add(Constants.AdminEmailId);
                        mail.MailSubject = "Campus Message|| " + msg.CampusProfile.CampusName;
                        mail.MailBody = "Campus Name: " + msg.CampusProfile.CampusName + " Dept: " + msg.CampusProfile.DepartmentName
                            + "<br /> Message ID : " + msg.MessageID + "<br /> Message: " + msg.Message1;
                        emailHelper.SendMail(mail);
                    }
                    catch (Exception)
                    {
                    }

                }
                return RedirectToAction("EducatorMessages", "Educator", new { ExpertID = messageVM.PostMessage.ExpertID });
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
        public ActionResult EducatorCampusFeedback(bool isFeedbackSent = false)
        {
            try
            {
                long CampusID = ViewBag.UserData.CampusID;

                List<WebinarViewModel> webinarList = _Context.CampusWebinarMappings.Where(x => x.CampusID == CampusID && x.WebinarFeedBacks.Count == 0 && x.Webinar.FromDateTime < DateTime.Now).Select(x => new WebinarViewModel()
                {
                    WebinarID = x.WebinarID,
                    WebinarName = x.Webinar.WebinarName,
                    WebinarSummary = x.Webinar.WebinarSummary,
                    FromDateTime = x.Webinar.FromDateTime,
                    Tags = x.Webinar.WebinarTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                    Participants = x.Webinar.Participants,

                    CampusWebinarID = x.CampusWebinarID,

                    FeedBacks = x.WebinarFeedBacks.Select(y => new WebinarFeedBackViewModel()
                    {
                        FeedBack = y.FeedBack
                    }).ToList()
                }).ToList();

                if (isFeedbackSent)
                {
                    TempData["feedbackSentMsg"] = Constants.FeedbackSentMsg;
                }

                return View(webinarList);
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
        public ActionResult EducatorCampusFeedback(List<WebinarViewModel> FeedbackList)
        {
            try
            {
                WebinarFeedBack wFeedback = new WebinarFeedBack()
                {
                    FeedBack = FeedbackList.FirstOrDefault().FeedBackMsg,
                    CampusWabinarID = FeedbackList.FirstOrDefault().CampusWebinarID,
                    //IsApproved = false,
                    SubmittedOn = DateTime.Now
                };
                _Context.WebinarFeedBacks.Add(wFeedback);
                _Context.SaveChanges();

                try
                {
                    WebinarFeedBack feedBack = _Context.WebinarFeedBacks
                        .Include(x => x.CampusWebinarMapping)
                        .Include(x => x.CampusWebinarMapping.CampusProfile)
                        .Include(x => x.CampusWebinarMapping.Webinar)
                        .Where(x => x.CampusWabinarID == wFeedback.CampusWabinarID).FirstOrDefault();
                    EmailViewModel mail = new EmailViewModel();
                    mail.MailToList = new List<string>();
                    mail.MailBCC = new List<string>();
                    mail.MailToList.Add(Constants.AdminEmailId);
                    mail.MailSubject = "Campus Feedback To Expert || " + wFeedback.CampusWebinarMapping.CampusProfile.CampusName;
                    mail.MailBody = "Campus Name: " + wFeedback.CampusWebinarMapping.CampusProfile.CampusName + " Dept: " + wFeedback.CampusWebinarMapping.CampusProfile.DepartmentName
                        + "<br /> Webinar Name : " + wFeedback.CampusWebinarMapping.Webinar.WebinarName + "<br /> Feedback message: " + wFeedback.FeedBack
                        + "<br /> Expert Name : " + wFeedback.CampusWebinarMapping.Webinar.ExpertProfile.FirstName + " " + wFeedback.CampusWebinarMapping.Webinar.ExpertProfile.LastName;
                    emailHelper.SendMail(mail);
                }
                catch (Exception)
                {
                }

                return RedirectToAction("EducatorCampusFeedback", "Educator", routeValues: new { isFeedbackSent = true });

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

        public ActionResult AttendClass(long WebinarID)
        {
            try
            {
                long CampusID = ViewBag.UserData.CampusID;

                CampusWebinarMapping cwMap = _Context.CampusWebinarMappings.Where(x => x.CampusID == CampusID && x.WebinarID == WebinarID).FirstOrDefault();
                if (cwMap != null)
                {
                    if (cwMap.IsApproved == null)
                    {
                        TempData["thnx"] = Constants.Thankyou;
                        TempData["msg"] = Constants.RequestAlreadyGenerated;
                    }
                    else if (cwMap.IsApproved == false)
                    {
                        TempData["thnx"] = Constants.Sorry;
                        TempData["msg"] = Constants.RequestRejected;
                    }
                    else
                    {
                        TempData["thnx"] = Constants.Congratulations;
                        TempData["msg"] = Constants.RequestAlreadyAccepted;
                    }
                }
                else
                {
                    CampusWebinarMapping campusWebinarMapping = new CampusWebinarMapping();
                    campusWebinarMapping.CampusID = CampusID;
                    campusWebinarMapping.WebinarID = WebinarID;
                    campusWebinarMapping.IsApproved = null;
                    _Context.CampusWebinarMappings.Add(campusWebinarMapping);
                    _Context.SaveChanges();

                    try
                    {
                        CampusWebinarMapping camWebMap = _Context.CampusWebinarMappings.Include(x => x.Webinar).Include(x => x.CampusProfile).Where(x => x.CampusWebinarID == campusWebinarMapping.CampusWebinarID).FirstOrDefault();
                        EmailViewModel mail = new EmailViewModel();
                        mail.MailBCC = new List<string>();
                        mail.MailToList = new List<string>();
                        mail.MailToList.Add(Constants.AdminEmailId);
                        mail.MailSubject = "Campus Request To Attend Upcoming Webinar || " + camWebMap.CampusProfile.CampusName;
                        mail.MailBody = "<br /> Campus Name: " + camWebMap.CampusProfile.CampusName + " Department Name: " + camWebMap.CampusProfile.DepartmentName
                        + "<br /> Webinar ID : " + camWebMap.WebinarID + "<br /> Webinar Name: " + camWebMap.Webinar.WebinarName
                        + "<br /> Webinar Date: " + camWebMap.Webinar.FromDateTime;
                        emailHelper.SendMail(mail);
                    }
                    catch (Exception)
                    {
                    }

                    TempData["thnx"] = Constants.Thankyou;
                    TempData["msg"] = Constants.RequestGenerated;
                }
                return RedirectToAction("WebinarDetails", "Home", routeValues: new { WebinarID = WebinarID });
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


        public ActionResult EducatorMasterClassRequest(long id, bool IsRequestSent = false)
        {
            try
            {
                CampusExpertRequestViewModel expertRequest = new CampusExpertRequestViewModel();
                ExpertProfile expert = _Context.ExpertProfiles.Find(id);
                ExpertProfileViewModel expertVM = new ExpertProfileViewModel()
                {
                    FirstName = expert.FirstName,
                    LastName = expert.LastName,
                    EmailID = expert.EmailID,
                    ExpertID = expert.ExpertID,
                    AboutMe = expert.AboutMe,
                    Company = expert.Company,
                    Position = expert.Position,
                    ExperienceYear = expert.ExperienceYear == null ? 0 : (int)expert.ExperienceYear,
                    ExperienceMonth = expert.ExperienceMonth == null ? 0 : (int)expert.ExperienceMonth,
                    ProfilePicPath = expert.ProfilePic.ProfilePicPath,
                    LinkedInProfileLink = expert.LinkedInProfileLink,
                    TweeterProfileLink = expert.TweeterProfileLink,
                    Tags = expert.ExpertTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                };
                expertRequest.ExpertProfile = expertVM;
                expertRequest.RequestedToID = id;
                expertRequest.ScheduleDate = DateTime.Now;
                expertRequest.IsRequestSent = IsRequestSent;
                return View(expertRequest);
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
        public ActionResult EducatorMasterClassRequest(CampusExpertRequestViewModel expertRequest)
        {
            try
            {
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    CampusExpertRequest campusExpertReq = new CampusExpertRequest();
                    campusExpertReq.CampusID = ViewBag.UserData.CampusID;
                    campusExpertReq.RequestedOn = DateTime.Now;
                    campusExpertReq.ScheduleDate = expertRequest.ScheduleDate;
                    campusExpertReq.RequestedToID = expertRequest.RequestedToID;
                    campusExpertReq.RequestMessage = expertRequest.RequestMessage;

                    _Context.CampusExpertRequests.Add(campusExpertReq);
                    _Context.SaveChanges();

                    try
                    {
                        CampusExpertRequest camExpertReq = _Context.CampusExpertRequests.Include(x => x.CampusProfile).Include(x => x.ExpertProfile).Where(x => x.RequestID == campusExpertReq.RequestID).FirstOrDefault();
                        EmailViewModel mail = new EmailViewModel();
                        mail.MailToList = new List<string>();
                        mail.MailBCC = new List<string>();
                        mail.MailToList.Add(Constants.AdminEmailId);
                        mail.MailSubject = "Campus Request of Master Class To Expert || " + camExpertReq.CampusProfile.CampusName;
                        mail.MailBody = "Expert Name: " + camExpertReq.ExpertProfile.FirstName + " " + camExpertReq.ExpertProfile.LastName
                        + "<br /> Campus Name: " + camExpertReq.CampusProfile.CampusName + " Department Name: " + camExpertReq.CampusProfile.DepartmentName
                        + "<br /> Request ID : " + camExpertReq.RequestID + "<br /> Request Message: " + camExpertReq.RequestMessage
                        + "<br /> Requested Schedule" + camExpertReq.ScheduleDate;
                        emailHelper.SendMail(mail);
                    }
                    catch (Exception)
                    {
                    }


                    return RedirectToAction("EducatorMasterClassRequest", "Educator", new { id = campusExpertReq.RequestedToID, IsRequestSent = true });
                }
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

        public ActionResult EducatorAttendedMasterClasses()
        {
            try
            {
                long CampusID = 0;
                if (ViewBag.UserData != null)
                {
                    CampusID = ViewBag.UserData.CampusID;
                }
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    List<WebinarViewModel> webinarList = _Context.Webinars.OrderByDescending(x => x.FromDateTime).Where(x => x.FromDateTime < DateTime.Now && x.CampusWebinarMappings.Select(y => y.CampusID).Contains(CampusID)).Select(x => new WebinarViewModel()
                    {
                        WebinarID = x.WebinarID,
                        WebinarName = x.WebinarName,
                        WebinarSummary = x.WebinarSummary,
                        FromDateTime = x.FromDateTime,
                        Tags = x.WebinarTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                        WebinarPicPath = x.ProfilePic.ProfilePicPath,
                        Campuses = x.CampusWebinarMappings.Select(y => new CampusViewModel()
                        {
                            CampusName = y.CampusProfile.CampusName,
                            DepartmentName = y.CampusProfile.DepartmentName,
                            ProfilePicPath = y.CampusProfile.ProfilePic.ProfilePicPath
                        }).ToList(),
                        Expert = x.ExpertID == null ? null : new ExpertProfileViewModel()
                        {
                            FirstName = x.ExpertProfile.FirstName,
                            LastName = x.ExpertProfile.LastName,
                            ExpertID = (long)x.ExpertID,
                            ProfilePicPath = x.ExpertProfile.ProfilePic.ProfilePicPath
                        }
                    }).ToList();
                    return View(webinarList);
                }

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
    }
}