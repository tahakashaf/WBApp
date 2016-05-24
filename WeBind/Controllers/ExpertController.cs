using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WeBind.Helper;
using WeBind.Models;
using System.Data.Entity;

namespace WeBind.Controllers
{
    [Authorize(Roles = "Expert")]
    public class ExpertController : BaseController
    {
        WeBindDemoEntities _Context = new WeBindDemoEntities();

        DateTimeHelper dateTimeHelper = null;
        EmailHelper emailHelper = null;
        public ExpertController()
        {
            dateTimeHelper = new DateTimeHelper();
            emailHelper = new EmailHelper();
        }
        // GET: Experts
        public ActionResult ExpertDashBoard()
        {
            try
            {
                long ExpertID = ViewBag.UserData.ExpertID;

                List<int> Participants = _Context.Webinars.Where(x => x.ExpertID == ExpertID).OrderBy(x => x.FromDateTime).Select(x => (int)x.Participants).ToList();
                string chartVal = "";
                for (int i = 0; i < Participants.Count; i++)
                {
                    chartVal += "[" + (i + 1) + "," + Participants[i] + "],";
                }

                chartVal = chartVal.Trim(new char[] { ',' });

                ExpertProfileViewModel expertDetails = _Context.ExpertProfiles.Where(x => x.ExpertID == ExpertID).Select(x => new ExpertProfileViewModel()
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    LastUpdated = x.LastUpdated,
                    EmailID = x.EmailID,
                    ExpertID = x.ExpertID,
                    ContactNo = x.ContactNo,
                    AboutMe = x.AboutMe,
                    LoginID = x.LoginID,
                    Password = x.Password,
                    ProfilePicID = x.ProfilePicID == null ? 5 : (long)x.ProfilePicID,
                    ProfilePicPath = x.ProfilePic.ProfilePicPath,
                    RegisteredOn = x.RegisteredOn,
                    Tags = x.ExpertTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                    ChartValue = chartVal,
                    Webinars = x.Webinars.OrderByDescending(y => y.FromDateTime).Where(y => y.FromDateTime < DateTime.Now).Take(5).Select(y => new WebinarViewModel()
                    {
                        WebinarID = y.WebinarID,
                        FromDateTime = y.FromDateTime,
                        WebinarName = y.WebinarName,
                        WebinarSummary = y.WebinarSummary,
                        WebinarDescription = y.WebinarDescription,
                        TimeDuration = y.TimeDuration,
                        Tags = y.WebinarTagMappings.Select(z => z.TagMaster.TagName).ToList(),
                        Campuses = y.CampusWebinarMappings.Select(z => new CampusViewModel()
                        {
                            CampusID = z.CampusID,
                            CampusName = z.CampusProfile.CampusName,
                            DepartmentName = z.CampusProfile.DepartmentName,
                            ProfilePicPath = z.CampusProfile.ProfilePic.ProfilePicPath
                        }).ToList(),
                        Participants = y.Participants
                    }).ToList()
                }).FirstOrDefault();

                return View(expertDetails);

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

        // GET: Expert/ExpertRequest/5
        public ActionResult ExpertRequest()
        {
            try
            {

                long ExpertID = ViewBag.UserData.ExpertID;

                List<CampusExpertRequestViewModel> campusReq = _Context.CampusExpertRequests.Where(x => x.RequestedToID == ExpertID && (bool)x.IsAdminApproved && x.IsExpertApproved == null).Select(x => new CampusExpertRequestViewModel()
                {

                    CampusID = x.CampusID,
                    CampusProfile = new CampusViewModel()
                    {
                        CampusName = x.CampusProfile.CampusName,
                        DepartmentName = x.CampusProfile.DepartmentName,
                        State = x.CampusProfile.State,
                        ProfilePicPath = x.CampusProfile.ProfilePic.ProfilePicPath
                    },
                    ScheduleDate = x.ScheduleDate,
                    RequestedOn = x.RequestedOn,
                    RequestMessage = x.RequestMessage,
                    RequestID = x.RequestID
                }).ToList();

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
        public ActionResult ExpertRequestResponse(long RequestID, bool IsExpertApproved)
        {
            long ExpertID = ViewBag.UserData.ExpertID;
            try
            {
                CampusExpertRequest campusreq = _Context.CampusExpertRequests.Find(RequestID);
                campusreq.IsExpertApproved = IsExpertApproved;

                _Context.Entry(campusreq).State = System.Data.Entity.EntityState.Modified;
                _Context.SaveChanges();

                try
                {
                    EmailViewModel mail = new EmailViewModel();
                    mail.MailToList = new List<string>();
                    mail.MailBCC = new List<string>();
                    mail.MailToList.Add(Constants.AdminEmailId);
                    if (IsExpertApproved)
                    {
                        mail.MailSubject = "Expert Response || Accepted Request";
                    }
                    else
                    {
                        mail.MailSubject = "Expert Response || Rejected Request";
                    }
                    mail.MailBody = "Expert Name: " + campusreq.ExpertProfile.FirstName + " " + campusreq.ExpertProfile.LastName
                        + "<br /> Campus Name: " + campusreq.CampusProfile.CampusName + "<br /> Department Name: " + campusreq.CampusProfile.DepartmentName
                        + "<br /> Request ID : " + campusreq.RequestID + "<br /> Request Message: " + campusreq.RequestMessage
                        + "<br /> Requested Schedule: " + campusreq.ScheduleDate;
                    emailHelper.SendMail(mail);

                    if (IsExpertApproved)
                    {
                        mail.MailToList.Add(campusreq.CampusProfile.EmailID);
                        emailHelper.SendMail(mail);
                    }
                }
                catch (Exception)
                {
                }

                return RedirectToAction("ExpertRequest", "Expert");
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

        // GET: Experts/Details/5
        public ActionResult ExpertProfile(bool IsProfileUpdated = false)
        {
            try
            {

                long ExpertID = ViewBag.UserData.ExpertID;
                ExpertProfileViewModel expertDetails = _Context.ExpertProfiles.Where(x => x.ExpertID == ExpertID).Select(x => new ExpertProfileViewModel()
                {
                    ExpertID = x.ExpertID,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Company = x.Company,
                    Position = x.Position,
                    ExperienceYear = x.ExperienceYear == null ? 0 : (int)x.ExperienceYear,
                    ExperienceMonth = x.ExperienceMonth == null ? 0 : (int)x.ExperienceMonth,
                    ExperienceYearList = Constants.ExperienceYears.ToList<int>(),
                    ExperienceMonthList = Constants.ExperienceMonths.ToList<int>(),
                    LinkedInProfileLink = x.LinkedInProfileLink,
                    TweeterProfileLink = x.TweeterProfileLink,
                    EmailID = x.EmailID,
                    ContactNo = x.ContactNo,
                    AboutMe = x.AboutMe,
                    LoginID = x.LoginID,
                    Password = x.Password,
                    ProfilePicID = x.ProfilePicID == null ? 0 : (long)x.ProfilePicID,
                    ProfilePicPath = x.ProfilePic.ProfilePicPath,
                    RegisteredOn = x.RegisteredOn,
                    Tags = x.ExpertTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                    Webinars = x.Webinars.OrderByDescending(y => y.FromDateTime).Take(5).Select(y => new WebinarViewModel()
                    {
                        WebinarID = y.WebinarID,
                        FromDateTime = y.FromDateTime,
                        WebinarName = y.WebinarName,
                        WebinarDescription = y.WebinarDescription,
                        Tags = y.WebinarTagMappings.Select(z => z.TagMaster.TagName).ToList(),
                    }).ToList(),
                    LastUpdated = x.LastUpdated,
                }).FirstOrDefault();

                expertDetails.MyTags = string.Join(",", expertDetails.Tags);
                expertDetails.AllTags = string.Join(",", _Context.TagMasters.Select(y => y.TagName));

                if (IsProfileUpdated)
                {
                    TempData["profileUpdated"] = Constants.ProfileDetailUpdatedMessage;
                }

                return View(expertDetails);
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

        // POST: Experts/Edit/5
        [HttpPost]
        public ActionResult ExpertProfile(ExpertProfileViewModel expertProfile, long expertId, HttpPostedFileBase file)
        {
            try
            {
                // TODO: Add update logic here

                List<ExpertTagMapping> expertTagMapping = new List<ExpertTagMapping>();
                if (expertProfile.MyTags != null)
                {
                    foreach (string tag in expertProfile.MyTags.Split(','))
                    {
                        TagMaster tagMaster = _Context.TagMasters.Where(x => x.TagName.ToLower() == tag.ToLower()).FirstOrDefault();
                        if (tagMaster == null)
                        {
                            TagMaster tagm = new TagMaster()
                            {
                                TagName = tag.ToUpper()
                            };
                            _Context.TagMasters.Add(tagm);
                            _Context.SaveChanges();
                            expertTagMapping.Add(new ExpertTagMapping() { ExpertID = expertId, TagID = tagm.TagID });
                        }
                        else
                        {
                            expertTagMapping.Add(new ExpertTagMapping() { ExpertID = expertId, TagID = tagMaster.TagID });
                        }
                    }
                }

                Models.ExpertProfile expert = _Context.ExpertProfiles.Find(expertId);

                _Context.ExpertTagMappings.RemoveRange(expert.ExpertTagMappings);
                _Context.SaveChanges();

                if (file != null)
                {
                    //file = dateTimeHelper.CropImage(file).Save(Path);
                    string serverPath = Constants.ExpertProfilePicBasePath;
                    if (!Directory.Exists(Server.MapPath(serverPath)))
                    {
                        Directory.CreateDirectory(Server.MapPath(serverPath));
                    }
                    string ext = Path.GetExtension(file.FileName);
                    string pic = expertProfile.ExpertID + ext;
                    string path = Path.Combine(Server.MapPath(serverPath), pic);
                    //file.SaveAs(path);
                    dateTimeHelper.CropImage(file).Save(path);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        file.InputStream.CopyTo(ms);
                        byte[] array = ms.GetBuffer();
                    }
                    if (expert.ProfilePicID == 1)
                    {
                        ProfilePic newpic = new ProfilePic() { ProfilePicPath = serverPath + "/" + pic, LastUpdated = DateTime.Now };
                        _Context.ProfilePics.Add(newpic);
                        _Context.SaveChanges();
                        expert.ProfilePicID = newpic.ProfilePicID;
                    }
                    else
                    {
                        expert.ProfilePic.ProfilePicPath = serverPath + "/" + pic;
                    }
                }

                expert.AboutMe = expertProfile.AboutMe;
                expert.EmailID = expertProfile.EmailID;
                expert.ExpertID = expertProfile.ExpertID;
                expert.FirstName = expertProfile.FirstName;
                expert.LastName = expertProfile.LastName;
                expert.ContactNo = expertProfile.ContactNo;
                expert.LinkedInProfileLink = expertProfile.LinkedInProfileLink;
                expert.TweeterProfileLink = expertProfile.TweeterProfileLink;
                expert.Company = expertProfile.Company;
                expert.Position = expertProfile.Position;
                expert.ExperienceYear = expertProfile.ExperienceYear;
                expert.ExperienceMonth = expertProfile.ExperienceMonth;
                expert.LastUpdated = DateTime.Now;
                expert.ExpertTagMappings = expertTagMapping;
                expert.ProfileCompleted = 20;

                if (!string.IsNullOrEmpty(expert.LastName))
                {
                    expert.ProfileCompleted += 10;
                }
                if (!string.IsNullOrEmpty(expert.LinkedInProfileLink))
                {
                    expert.ProfileCompleted += 10;
                }
                if (!string.IsNullOrEmpty(expert.Company))
                {
                    expert.ProfileCompleted += 10;
                }
                if (!string.IsNullOrEmpty(expert.Position))
                {
                    expert.ProfileCompleted += 10;
                }
                if (!string.IsNullOrEmpty(expert.ContactNo))
                {
                    expert.ProfileCompleted += 10;
                }
                if (expert.ExperienceYear != null && expert.ExperienceYear != 0 || expert.ExperienceMonth != null && expert.ExperienceMonth != 0)
                {
                    expert.ProfileCompleted += 10;
                }
                if (expert.ExpertTagMappings.Count > 0)
                {
                    expert.ProfileCompleted += 10;
                }
                if (expert.ProfilePicID != 1)
                {
                    expert.ProfileCompleted += 10;
                }

                _Context.Entry(expert).State = System.Data.Entity.EntityState.Modified;
                _Context.SaveChanges();

                return RedirectToAction("ExpertProfile", "Expert", routeValues: new { IsProfileUpdated = true });
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

        // GET: Experts/Create
        public ActionResult ExpertWebinarsList()
        {
            try
            {
                long ExpertID = ViewBag.UserData.ExpertID;
                List<WebinarViewModel> webinarList = _Context.Webinars.OrderByDescending(x => x.FromDateTime).Where(x => x.ExpertID == ExpertID).Select(x => new WebinarViewModel()
                {
                    WebinarID = x.WebinarID,
                    WebinarName = x.WebinarName,
                    WebinarDescription = x.WebinarDescription,
                    WebinarSummary = x.WebinarSummary,
                    FromDateTime = x.FromDateTime,
                    WebinarPicPath = x.ProfilePic.ProfilePicPath,
                    Tags = x.WebinarTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                    Participants = x.Participants,
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

        // GET: Experts/Create

        public ActionResult ExpertMessages(long CampusID = 0)
        {
            try
            {
                long ExpertID = ViewBag.UserData.ExpertID;

                List<CampusMessagesViewModel> campusMsgVM = _Context.Messages.Where(x => x.ExpertID == ExpertID).GroupBy(x => x.CampusID).Select(x => new CampusMessagesViewModel()
                {
                    CampusID = x.OrderByDescending(y => y.MessageDateTime).Select(y => y.CampusProfile.CampusID).FirstOrDefault(),
                    CampusName = x.OrderByDescending(y => y.MessageDateTime).Select(y => y.CampusProfile.CampusName).FirstOrDefault(),
                    DepartmentName = x.OrderByDescending(y => y.MessageDateTime).Select(y => y.CampusProfile.DepartmentName).FirstOrDefault(),
                    LastMessage = x.OrderByDescending(y => y.MessageDateTime).Select(y => y.Message1).FirstOrDefault(),
                    //MessageAgoDateTime = dateTimeHelper.GetAgoTime(x.OrderByDescending(y => y.MessageDateTime).Select(y => y.MessageDateTime).FirstOrDefault()),
                    MessageDateTime = x.OrderByDescending(y => y.MessageDateTime).Select(y => y.MessageDateTime).FirstOrDefault(),
                    ProfilePicPath = x.OrderByDescending(y => y.MessageDateTime).Select(y => y.CampusProfile.ProfilePic.ProfilePicPath).FirstOrDefault()
                }).ToList();

                campusMsgVM = campusMsgVM.OrderByDescending(x => x.MessageDateTime).Select(x => new CampusMessagesViewModel()
                {
                    MessageAgoDateTime = dateTimeHelper.GetAgoTime(x.MessageDateTime),
                    CampusID = x.CampusID,
                    CampusName = x.CampusName,
                    DepartmentName = x.DepartmentName,
                    LastMessage = x.LastMessage,
                    MessageDateTime = x.MessageDateTime,
                    ProfilePicPath = x.ProfilePicPath
                }).ToList();

                List<MessageContentViewModel> messageContentVM = new List<MessageContentViewModel>();
                if (campusMsgVM.Count > 0 && CampusID == 0)
                {
                    CampusID = campusMsgVM[0].CampusID;
                }
                messageContentVM = GetCampusMessages(ExpertID, CampusID);

                MessageViewModel msgVM = new MessageViewModel()
                {
                    PostMessage = new MessageContentViewModel()
                    {
                        CampusID = CampusID
                    },
                    Campuses = campusMsgVM,
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

        public ActionResult ExpertCampusFeedback()
        {
            try
            {
                long ExpertID = ViewBag.UserData.ExpertID;

                List<WebinarViewModel> webinarList = _Context.Webinars.OrderByDescending(x => x.FromDateTime).Where(x => x.FromDateTime < DateTime.Now && x.ExpertID == ExpertID).Select(x => new WebinarViewModel()
                {
                    WebinarID = x.WebinarID,
                    WebinarName = x.WebinarName,
                    WebinarSummary = x.WebinarSummary,
                    FromDateTime = x.FromDateTime,
                    Tags = x.WebinarTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                    Participants = x.Participants,
                    Campuses = x.CampusWebinarMappings.Select(y => new CampusViewModel()
                    {
                        CampusName = y.CampusProfile.CampusName,
                        ProfilePicPath = y.CampusProfile.ProfilePic.ProfilePicPath
                    }).ToList(),
                    FeedBacks = x.CampusWebinarMappings.Where(y => y.WebinarFeedBacks.Where(z => z.IsApproved == true).ToList().Count != 0).Select(y => new WebinarFeedBackViewModel()
                    {
                        FeedBack = y.WebinarFeedBacks.Where(z => z.IsApproved == true).FirstOrDefault().FeedBack,
                        SubmittedOn = y.WebinarFeedBacks.Where(z => z.IsApproved == true).FirstOrDefault().SubmittedOn,
                        CampusName = y.WebinarFeedBacks.Where(z => z.IsApproved == true).FirstOrDefault().CampusWebinarMapping.CampusProfile.CampusName,
                        ProfilePicPath = y.WebinarFeedBacks.Where(z => z.IsApproved == true).FirstOrDefault().CampusWebinarMapping.CampusProfile.ProfilePic.ProfilePicPath,
                        IsApproved = y.WebinarFeedBacks.FirstOrDefault().IsApproved == true ? true : false
                    }).ToList()

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

        private List<MessageContentViewModel> GetCampusMessages(long ExpertID, long CampusID)
        {
            try
            {
                List<MessageContentViewModel> messageContentVM = _Context.Messages.OrderByDescending(x => x.MessageDateTime).Where(x => x.ExpertID == ExpertID && x.CampusID == CampusID && x.IsAdminApproved == true).Select(x => new MessageContentViewModel()
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

        // POST: Experts/Edit/5
        [HttpPost]
        public ActionResult PostMessage(MessageViewModel messageVM)
        {
            try
            {

                if (messageVM.PostMessage.Message != null && messageVM.PostMessage.Message.Trim() != "")
                {
                    Message message = new Message()
                    {
                        CampusID = messageVM.PostMessage.CampusID,
                        ExpertID = ViewBag.UserData.ExpertID,
                        IsFromExpert = true,
                        Message1 = messageVM.PostMessage.Message.Trim(),
                        MessageDateTime = DateTime.Now,
                        IsAdminApproved = true
                    };
                    _Context.Messages.Add(message);
                    _Context.SaveChanges();

                    try
                    {
                        Message msg = _Context.Messages.Include(x => x.ExpertProfile).Include(x => x.CampusProfile).Where(x => x.MessageID == message.MessageID).FirstOrDefault();
                        EmailViewModel mail = new EmailViewModel();
                        mail.MailToList = new List<string>();
                        mail.MailBCC = new List<string>();
                        mail.MailBCC.Add(Constants.AdminEmailId);
                        mail.MailToList.Add(msg.CampusProfile.EmailID);
                        mail.MailSubject = "WeBind || Message From Expert || " + msg.ExpertProfile.FirstName + " " + msg.ExpertProfile.LastName;
                        mail.MailBody = "Hi <br /><br /> You have one new message from " + msg.ExpertProfile.FirstName + " " + msg.ExpertProfile.LastName
                            + "<br /><br />Message: " + msg.Message1;
                        emailHelper.SendMail(mail);
                    }
                    catch (Exception)
                    {
                    }
                }
                return RedirectToAction("ExpertMessages", "Expert", new { CampusID = messageVM.PostMessage.CampusID });
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


        // Get: Experts/Edit/5
        public ActionResult ExpertReferFriend()
        {
            try
            {
                RecommandAFriendViewModel recFriend = new RecommandAFriendViewModel()
                {
                    Message = Constants.ReferAFriendDefaultMessgae
                };
                return View(recFriend);
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

        // Post: Experts/Edit/5
        [HttpPost]
        public ActionResult ExpertReferFriend(RecommandAFriendViewModel recFriendVM)
        {
            try
            {
                long ExpertID = ViewBag.UserData.ExpertID;

                RecommandAFriend recFriend = new RecommandAFriend()
                {
                    Message = recFriendVM.Message,
                    ExpertID = ExpertID,
                    FriendEmailIDs = recFriendVM.FriendEmailIDs,
                    RecommandedOn = DateTime.Now
                };

                _Context.RecommandAFriends.Add(recFriend);
                _Context.SaveChanges();

                TempData["referFriendSentMsg"] = Constants.ReferAFriendSentMessage;
                try
                {
                    RecommandAFriend recAFrnd = _Context.RecommandAFriends.Include(x => x.ExpertProfile).Where(x => x.RecommandationID == recFriend.RecommandationID).FirstOrDefault();
                    EmailViewModel email = new EmailViewModel();
                    email.MailToList = new List<string>();
                    email.MailToList = recAFrnd.FriendEmailIDs.Split(',').ToList();
                    email.MailBCC = new List<string>();
                    email.MailBCC.Add(Constants.AdminEmailId);
                    email.MailSubject = "WeBind || Refer A Friend";
                    email.MailBody = "You are refered by " + recAFrnd.ExpertProfile.FirstName + " " + recAFrnd.ExpertProfile.LastName + "\n<br /><br />Message: " + recAFrnd.Message;
                    emailHelper.SendMail(email);
                }
                catch (Exception)
                {
                }

                return View(recFriendVM);
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
