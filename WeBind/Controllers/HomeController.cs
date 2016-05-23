using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WeBind.Helper;
using WeBind.Models;

namespace WeBind.Controllers
{
    public class HomeController : BaseExpertController
    {
        EmailHelper emailHelper = null;
        public HomeController()
        {
            emailHelper = new EmailHelper();
        }
        public static readonly Regex YoutubeVideoRegex = new Regex("youtu(?:\\.be|be\\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)");
        public ActionResult Index()
        {
            using (WeBindDemoEntities _Context = new WeBindDemoEntities())
            {

                List<WebinarViewModel> upcomingMC = _Context.Webinars.OrderBy(x => x.FromDateTime).Where(x => x.FromDateTime > DateTime.Now).Select(x => new WebinarViewModel()
                {
                    WebinarID = x.WebinarID,
                    WebinarName = x.WebinarName,
                    WebinarSummary = x.WebinarSummary,
                    FromDateTime = x.FromDateTime,
                    Tags = x.WebinarTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                    WebinarPicPath = x.ProfilePic.ProfilePicPath,                    
                    Expert = x.ExpertID == null ? null : new ExpertProfileViewModel()
                    {
                        FirstName =  x.ExpertProfile.FirstName,
                        LastName = x.ExpertProfile.LastName,
                        ExpertID = (long)x.ExpertID,
                        ProfilePicPath = x.ExpertProfile.ProfilePic.ProfilePicPath
                    }
                }).ToList();

                ViewData.Add("upcomingmc", upcomingMC);

                List<WebinarFeedBackViewModel> feedbacks = _Context.WebinarFeedBacks.OrderByDescending(x => x.SubmittedOn).Where(x => x.IsApproved == true).Take(3).Select(x => new WebinarFeedBackViewModel()
                {
                    CampusName = x.CampusWebinarMapping.CampusProfile.CampusName,
                    DepartmentName = x.CampusWebinarMapping.CampusProfile.DepartmentName,
                    FeedBack = x.FeedBack,
                    ProfilePicPath = x.CampusWebinarMapping.CampusProfile.ProfilePic.ProfilePicPath
                }).ToList();

                ViewData.Add("feedbacks", feedbacks);

                List<CampusViewModel> partnerColleges = _Context.CampusProfiles.OrderBy(x => x.RegisteredOn).Where(x => x.IsPartnerCollege).Select(x => new CampusViewModel()
                {
                    CampusName = x.CampusName,
                    DepartmentName = x.DepartmentName,
                    ProfilePicPath = x.ProfilePic.ProfilePicPath
                }).ToList();

                ViewData.Add("partnerColleges", partnerColleges);

                return View();
            }
        }

        public ActionResult CampusSuperHero()
        {
            return View();
        }

        public ActionResult WhatIsMasterClass()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult PublicProfile(long ExpertID)
        {
            try
            {
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {

                    ExpertProfile expert = _Context.ExpertProfiles.Find(ExpertID);
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
                        Webinars = expert.Webinars.OrderByDescending(y => y.FromDateTime).Take(5).Select(y => new WebinarViewModel()
                        {
                            WebinarID = y.WebinarID,
                            FromDateTime = y.FromDateTime,
                            WebinarName = y.WebinarName,
                            WebinarSummary = y.WebinarSummary,
                            WebinarDescription = y.WebinarDescription,
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
                    };

                    return View(expertVM);
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
                    mail.MailBody = "Hi <br /><br /> Exception : " + ex.StackTrace;
                    emailHelper.SendMail(mail);
                }
                catch (Exception)
                {
                }
                return RedirectToAction("Error", "Home");
            }
        }
        public ActionResult WebinarDetails(long WebinarID)
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


                    Webinar webinar = _Context.Webinars.Find(WebinarID);


                    Match youtubeMatch = null;
                    if (webinar.YoutubeUrl != null)
                    {
                        youtubeMatch = YoutubeVideoRegex.Match(webinar.YoutubeUrl);
                    }

                    string id = null;

                    if (youtubeMatch != null && youtubeMatch.Success)
                        id = youtubeMatch.Groups[1].Value;

                    WebinarViewModel webinarViewModel = new WebinarViewModel()
                    {
                        WebinarID = webinar.WebinarID,
                        WebinarName = webinar.WebinarName,
                        WebinarDescription = webinar.WebinarDescription,
                        WebinarSummary = webinar.WebinarSummary,
                        WhatWillYouLearn = webinar.WhatWillYouLearn,
                        FromDateTime = webinar.FromDateTime,
                        TimeDuration = webinar.TimeDuration,
                        Tags = webinar.WebinarTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                        Participants = webinar.Participants,
                        YoutubeUrl = id,
                        IsWebinarRequested = webinar.CampusWebinarMappings.Where(y => y.CampusID == CampusID).FirstOrDefault() == null ? false : true,
                        Campuses = webinar.CampusWebinarMappings.Where(x => x.IsApproved == true).Select(z => new CampusViewModel()
                        {
                            CampusID = z.CampusID,
                            CampusName = z.CampusProfile.CampusName,
                            DepartmentName = z.CampusProfile.DepartmentName,
                            ProfilePicPath = z.CampusProfile.ProfilePic.ProfilePicPath
                        }).ToList(),
                        Expert = webinar.ExpertID == null ? null : new ExpertProfileViewModel()
                        {
                            FirstName = webinar.ExpertProfile.FirstName,
                            LastName = webinar.ExpertProfile.LastName,
                            ExpertID = (long)webinar.ExpertID,
                            ProfilePicPath = webinar.ExpertProfile.ProfilePic.ProfilePicPath,
                            AboutMe = webinar.ExpertProfile.AboutMe
                        },
                        Brand = webinar.BrandID == null ? null : new BrandViewModel()
                        {
                            BrandID = webinar.BrandProfile.BrandID,
                            BrandLink = webinar.BrandProfile.BrandLink,
                            ProfilePicPath = webinar.BrandProfile.ProfilePic.ProfilePicPath,
                            YoutubeURL = webinar.BrandProfile.YoutubeURL,
                            BrandContent = webinar.BrandProfile.BrandContent,
                            BrandName = webinar.BrandProfile.BrandName,
                            ContactNo = webinar.BrandProfile.ContactNo,
                            BrandBannerImages = webinar.BrandProfile.BrandBannerImages
                        }
                    };
                    return View(webinarViewModel);
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
                    mail.MailBody = "Hi <br /><br /> Exception : " + ex.StackTrace;
                    emailHelper.SendMail(mail);
                }
                catch (Exception)
                {
                }
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult EliteMasters()
        {
            try
            {
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    List<ExpertProfileViewModel> masters = _Context.ExpertProfiles.Where(x => x.IsEliteMaster).Select(x => new ExpertProfileViewModel()
                    {
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        ExpertID = x.ExpertID,
                        AboutMe = x.AboutMe,
                        ProfilePicPath = x.ProfilePic.ProfilePicPath,
                        Company = x.Company,
                        Position = x.Position,
                        Tags = x.ExpertTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                        Webinars = x.Webinars.OrderByDescending(y => y.FromDateTime).Take(1).Select(y => new WebinarViewModel()
                        {
                            WebinarID = y.WebinarID,
                            FromDateTime = y.FromDateTime,
                            WebinarName = y.WebinarName,
                            WebinarSummary = y.WebinarSummary,
                        }).ToList()

                    }).ToList();
                    return View(masters);
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
                    mail.MailBody = "Hi <br /><br /> Exception : " + ex.StackTrace;
                    emailHelper.SendMail(mail);
                }
                catch (Exception)
                {
                }
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult MastersList(string tag = null)
        {
            try
            {
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    List<ExpertProfileViewModel> masters = _Context.ExpertProfiles.Where(x => x.IsConfirmed && (x.ExpertTagMappings.Select(y => y.TagMaster.TagName).Any(s => s.Contains(tag)) || tag == null)).Select(x => new ExpertProfileViewModel()
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    ExpertID = x.ExpertID,
                    AboutMe = x.AboutMe,
                    ProfilePicPath = x.ProfilePic.ProfilePicPath,
                    Company = x.Company,
                    Position = x.Position,
                    Tags = x.ExpertTagMappings.Select(y => y.TagMaster.TagName).ToList(),

                }).ToList();
                    return View(masters);
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
                    mail.MailBody = "Hi <br /><br /> Exception : " + ex.StackTrace;
                    emailHelper.SendMail(mail);
                }
                catch (Exception)
                {
                }
                return RedirectToAction("Error", "Home");
            }

        }

        public ActionResult PreRecordedMCList()
        {
            try
            {
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    List<WebinarViewModel> webinarList = _Context.Webinars.OrderBy(x => x.FromDateTime).Where(x => x.FromDateTime < DateTime.Now).Select(x => new WebinarViewModel()
                    {
                        WebinarID = x.WebinarID,
                        WebinarName = x.WebinarName,
                        WebinarDescription = x.WebinarDescription,
                        WebinarSummary = x.WebinarSummary,
                        FromDateTime = x.FromDateTime,
                        Tags = x.WebinarTagMappings.Select(y => y.TagMaster.TagName).ToList(),
                        Participants = x.Participants,
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
                            Company = x.ExpertProfile.Company,
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
            }
            catch (Exception ex)
            {
                try
                {
                    EmailViewModel mail = new EmailViewModel();
                    mail.MailToList = new List<string>();
                    mail.MailBCC = new List<string>();
                    mail.MailSubject = "WeBind || Error";
                    mail.MailBody = "Hi <br /><br /> Exception : " + ex.StackTrace;
                    emailHelper.SendMail(mail);
                }
                catch (Exception)
                {
                }
                return RedirectToAction("Error", "Home");
            }
        }
        public ActionResult OurPartnerColleges()
        {
            try
            {
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    List<CampusViewModel> partnerColleges = _Context.CampusProfiles.OrderBy(x => x.RegisteredOn).Where(x => x.IsPartnerCollege).Select(x => new CampusViewModel()
                    {
                        CampusName = x.CampusName,
                        DepartmentName = x.DepartmentName,
                        ProfilePicPath = x.ProfilePic.ProfilePicPath,
                        District = x.District,
                        State = x.State
                    }).ToList();

                    return View(partnerColleges);
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
                    mail.MailBody = "Hi <br /><br /> Exception : " + ex.StackTrace;
                    emailHelper.SendMail(mail);
                }
                catch (Exception)
                {
                }
                return RedirectToAction("Error", "Home");
            }
        }
        public ActionResult TheConcept()
        {
            return View();
        }
        public ActionResult ContactUs()
        {
            return View();
        }
        public ActionResult OurTeam()
        {
            return View();
        }
        public ActionResult Career()
        {
            return View();
        }
        public ActionResult Blog()
        {
            return View();
        }

        public ActionResult NewsLetter(string newsletter)
        {
            try
            {
                EmailViewModel mail = new EmailViewModel();
                mail.MailToList = new List<string>() { System.Configuration.ConfigurationManager.AppSettings["newsletter_emailID"] };
                mail.MailBCC = new List<string>();
                mail.MailSubject = "WeBind | News Letter";
                mail.MailBody = "Hi Webind Team,<br /><br /><b>" + newsletter + "</b> has subscribed for the News Letter.";
                emailHelper.SendMail(mail);
            }
            catch (Exception)
            {
            }

            return RedirectToAction("Index");
        }
    }
}