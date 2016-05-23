using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeBind.Helper;
using WeBind.Models;

namespace WeBind.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : BaseExpertController
    {
        WeBindDemoEntities _Context = new WeBindDemoEntities();
        DateTimeHelper dateTimeHelper = null;
        EmailHelper emailHelper = null;

        public StudentController()
        {
            dateTimeHelper = new DateTimeHelper();
            emailHelper = new EmailHelper();
        }

        // GET: Student
        public ActionResult Index()
        {
            return View();
        }

        // GET: Student
        public ActionResult StudentDashBoard()
        {
            try
            {
                long StudentID = ViewBag.UserData.StudentID;

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
                    //IsWebinarRequested = x.CampusWebinarMappings.Where(y => y.CampusID == CampusID).FirstOrDefault() == null ? false : true,
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

        public ActionResult StudentWebinarsList()
        {
            try
            {
                long StudentID = ViewBag.UserData.StudentID;
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
                            CampusID = y.CampusID,
                            CampusName = y.CampusProfile.CampusName,
                            DepartmentName = y.CampusProfile.DepartmentName,
                            ProfilePicPath = y.CampusProfile.ProfilePic.ProfilePicPath,
                            District = y.CampusProfile.District,
                            State = y.CampusProfile.State,
                            ProfessorName = y.CampusProfile.ProfessorName
                        }).ToList(),
                        Expert = x.ExpertID == null ? null : new ExpertProfileViewModel()
                        {
                            FirstName = x.ExpertProfile.FirstName,
                            LastName = x.ExpertProfile.LastName,
                            Company = x.ExpertProfile.Company,
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

        public ActionResult WebinarQuestions(long WebinarID, long CampusID)
        {
            try
            {
                long StudentID = ViewBag.UserData.StudentID;
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    Webinar selectedWebinar = _Context.Webinars.Find(WebinarID);
                    WebinarViewModel webinar = new WebinarViewModel();
                    webinar.WebinarName = selectedWebinar.WebinarName;
                    webinar.WebinarID = selectedWebinar.WebinarID;
                    List<CertificateQueViewModel> certificateQues = selectedWebinar.WebinarQuestions.Select(x => new CertificateQueViewModel()
                    {
                        QuestionID = x.QuestionID,
                        Question = x.Question,
                        WebinarID = x.WebinarID,
                        Webinar = new WebinarViewModel()
                        {
                            WebinarName = selectedWebinar.WebinarName,
                            WebinarPicPath = selectedWebinar.ProfilePic.ProfilePicPath
                        },
                        WebinarAnswers = x.WebinarAnswers.Select(y => new CertificateAnswersViewModel()
                        {
                            AnswerID = y.AnswerID,
                            Answer = y.Answer,
                            QuestionID = y.QuestionID,
                            IsCorrect = y.IsCorrect,
                            IsSelected = true
                        }).ToList()
                    }).ToList();
                    webinar.WebinarQuestions = certificateQues;

                    return View(webinar);
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

        public ActionResult Certificate()
        {
            try
            {
                long StudentID = ViewBag.UserData.StudentID;
                return null;
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

        public ActionResult StudentBlogs()
        {
            try
            {
                long StudentID = ViewBag.UserData.StudentID;
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    List<StudentBlogViewModel> StudentBlogs = _Context.StudentBlogs.Where(x => x.IsApproved == true || x.StudentID == StudentID).Select(x => new StudentBlogViewModel()
                    {
                        Blog = x.Blog,
                        BlogTitle = x.BlogTitle,
                        CreationDate = x.CreationDate,
                        IsApproved = x.IsApproved,
                        StudentBlogID = x.StudentBlogID,
                        StudentID = x.StudentID,
                        IsWrittenByMe = StudentID == x.StudentID,
                        StudentProfile = new StudentViewModel()
                        {
                            StudentName = x.StudentProfile.StudentName,
                            ProfilePicPath = x.StudentProfile.ProfilePic.ProfilePicPath,
                        },
                        UpdationDate = x.UpdationDate
                    }).ToList();
                    return View(StudentBlogs);
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

        public ActionResult StudentCreateBlog()
        {
            try
            {
                long StudentID = ViewBag.UserData.StudentID;
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    StudentBlogViewModel StudentBlog = new StudentBlogViewModel()
                    {
                        StudentID = StudentID,
                    };
                    return View(StudentBlog);
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

        public ActionResult StudentUpdateBlog(int id)
        {
            try
            {
                long StudentID = ViewBag.UserData.StudentID;
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    StudentBlog studentBlog = _Context.StudentBlogs.Find(id);
                    if (studentBlog.StudentID != StudentID)
                    {
                        return null;
                    }
                    StudentBlogViewModel StudentBlog = new StudentBlogViewModel()
                    {
                        StudentBlogID = studentBlog.StudentBlogID,
                        StudentID = StudentID,
                        Blog = studentBlog.Blog,
                        BlogTitle = studentBlog.BlogTitle,
                    };
                    return View(StudentBlog);
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

        public ActionResult StudentShowBlog(int id)
        {
            try
            {
                long StudentID = ViewBag.UserData.StudentID;
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    StudentBlog studentBlog = _Context.StudentBlogs.Find(id);
                    StudentBlogViewModel StudentBlog = new StudentBlogViewModel()
                    {
                        StudentBlogID = studentBlog.StudentBlogID,
                        StudentID = StudentID,
                        Blog = studentBlog.Blog,
                        BlogTitle = studentBlog.BlogTitle,
                        IsApproved = studentBlog.IsApproved,
                        UpdationDate = studentBlog.UpdationDate,
                        CreationDate = studentBlog.CreationDate,
                        StudentProfile = new StudentViewModel()
                         {
                             StudentName = studentBlog.StudentProfile.StudentName,
                             ProfilePicPath = studentBlog.StudentProfile.ProfilePic.ProfilePicPath
                         }
                    };
                    return View(StudentBlog);
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


        public ActionResult PostBlog(int id, StudentBlogViewModel studentBlogVM)
        {
            try
            {
                long StudentID = ViewBag.UserData.StudentID;
                using (WeBindDemoEntities _Context = new WeBindDemoEntities())
                {
                    StudentBlog studentBlog = new StudentBlog();
                    if (id == 0)
                    {
                        studentBlog.BlogTitle = studentBlogVM.BlogTitle;
                        studentBlog.Blog = studentBlogVM.Blog;
                        studentBlog.IsApproved = null;
                        studentBlog.UpdationDate = DateTime.Now;
                        studentBlog.StudentID = StudentID;
                        studentBlog.CreationDate = DateTime.Now;
                        _Context.StudentBlogs.Add(studentBlog);
                    }
                    else
                    {
                        studentBlog = _Context.StudentBlogs.Find(id);
                        studentBlog.BlogTitle = studentBlogVM.BlogTitle;
                        studentBlog.Blog = studentBlogVM.Blog;
                        studentBlog.IsApproved = null;
                        studentBlog.UpdationDate = DateTime.Now;
                        _Context.Entry(studentBlog).State = System.Data.Entity.EntityState.Modified;
                    }
                    _Context.SaveChanges();
                    return RedirectToAction("StudentBlogs", "Student");
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