using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using WeBind.Models;
using WeBind.Helper;

namespace WeBind.Controllers
{

    [Authorize]
    public class AccountController : BaseExpertController
    {
        WeBindDemoEntities _Context = new WeBindDemoEntities();
        private ApplicationUserManager _userManager;
        EmailHelper emailHelper = null;
        public AccountController()
        {
            emailHelper = new EmailHelper();
        }

        public AccountController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl, string RoleID)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel() { RoleId = RoleID });
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl, string RoleId)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.Email, model.Password);
                if (user != null && user.Roles.FirstOrDefault().RoleId == model.RoleId)
                {
                    if (user.EmailConfirmed)
                    {
                        await SignInAsync(user, model.RememberMe);
                        if ((int)Roles.Expert == int.Parse(model.RoleId))
                        {
                            return RedirectToMasterDashboard(returnUrl, _Context.ExpertProfiles.Where(x => x.EmailID == user.Email).Select(x => x.ExpertID).FirstOrDefault());
                        }
                        else if ((int)Roles.Educator == int.Parse(model.RoleId))
                        {
                            return RedirectToCampusDashboard(returnUrl, _Context.CampusProfiles.Where(x => x.EmailID == user.Email).Select(x => x.CampusID).FirstOrDefault());
                        }
                        else 
                        {
                            return RedirectToStudentDashboard(returnUrl, _Context.StudentProfiles.Where(x => x.EmailID == user.Email).Select(x => x.StudentID).FirstOrDefault());
                        }
                    }
                    else
                    {
                        return RedirectToAction("RegisterConfirmation", "Account");
                    }
                }
                else
                {
                    ModelState.AddModelError("invalidLogin", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register(string RoleName)
        {
            RegisterViewModel registerVM = new RegisterViewModel();
            registerVM.RoleName = RoleName;
            return View(registerVM);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model, string RoleName)
        {
            if (model.RoleName.Equals(Roles.Expert.ToString()))
            {
                if (model.FirstName == null)
                {
                    ModelState.AddModelError("FirstName", "firstname required");
                }
                //if (model.LastName == null)
                //{
                //    ModelState.AddModelError("LastName", "lastname required");
                //}
            }
            if (model.RoleName.Equals(Roles.Educator.ToString()))
            {
                if (model.CampusName == null)
                {
                    ModelState.AddModelError("campus", "Campus Name is required");
                }
                if (model.DepartmentName == null)
                {
                    ModelState.AddModelError("dept", "Department Name is required");
                }
                if (model.ProfessorName == null)
                {
                    ModelState.AddModelError("profName", "Professor Name is Required");
                }
                if (model.ContactNo == null)
                {
                    ModelState.AddModelError("mbleNo", "Mobile Number is Required");
                }
            }
            if (model.RoleName.Equals(Roles.Student.ToString()))
            {
                if (model.StudentName == null)
                {
                    ModelState.AddModelError("campus", "Campus Name is required");
                }
                if (model.DepartmentName == null)
                {
                    ModelState.AddModelError("dept", "Department Name is required");
                }
                if (model.CampusName == null)
                {
                    ModelState.AddModelError("profName", "Professor Name is Required");
                }
                if (model.ContactNo == null)
                {
                    ModelState.AddModelError("mbleNo", "Mobile Number is Required");
                }
                if (model.Year == null)
                {
                    ModelState.AddModelError("mbleNo", "Mobile Number is Required");
                }
            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email, PhoneNumber = model.ContactNo };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var currentUser = UserManager.FindByName(user.UserName);
                    UserManager.AddToRole(currentUser.Id, model.RoleName);
                    //await SignInAsync(user, isPersistent: false);

                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                    if (model.RoleName.Equals(Roles.Expert.ToString()))
                    {
                        ExpertProfile expert = new ExpertProfile()
                        {
                            FirstName = model.FirstName,
                            LastName = model.LastName,
                            EmailID = model.Email,
                            Password = model.Password,
                            LoginID = model.Email,
                            RegisteredOn = DateTime.Now,
                            IsConfirmed = false,
                            ProfileCompleted = 10,
                            ProfilePicID = 1,
                            IsEliteMaster = false,
                            ContactNo = model.ContactNo
                        };
                        _Context.ExpertProfiles.Add(expert);
                        _Context.SaveChanges();

                        try
                        {
                            EmailTemplate emailTemplate = _Context.EmailTemplates.Find(1);
                            EmailViewModel email = new EmailViewModel();
                            email.MailSubject = emailTemplate.EmailSubject;
                            email.MailBody = emailTemplate.EmailTemplate1.Replace(Helper.Constants.PL_ExpertName, expert.FirstName).Replace(Helper.Constants.PL_CallBackUrl, callbackUrl).Replace(Helper.Constants.PL_Twitter, Helper.Constants.TwitterLink).Replace(Helper.Constants.PL_Facebook, Helper.Constants.FacebookLink).Replace(Helper.Constants.PL_WebindLink, Helper.Constants.WebindLink).Replace(Helper.Constants.PL_WebindMail, Helper.Constants.WebinMail);
                            email.MailToList = new List<string>();
                            email.MailBCC = new List<string>();
                            email.MailToList.Add(user.Email);
                            emailHelper.SendMail(email);
                            UserManager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        }
                        catch (Exception e)
                        {
                            return View("Error", "Home");
                        }

                    }
                    else if (model.RoleName.Equals(Roles.Educator.ToString()))
                    {
                        CampusProfile campus = new CampusProfile()
                        {
                            CampusName = model.CampusName,
                            DepartmentName = model.DepartmentName,
                            ProfessorName = model.ProfessorName,
                            EmailID = model.Email,
                            Password = model.Password,
                            LoginID = model.Email,
                            RegisteredOn = DateTime.Now,
                            IsConfirmed = false,
                            ProfilePicID = 2,
                            IsPartnerCollege = false,
                            ContactNo = model.ContactNo
                        };
                        _Context.CampusProfiles.Add(campus);
                        _Context.SaveChanges();

                        try
                        {
                            EmailTemplate emailTemplate = _Context.EmailTemplates.Find(2);
                            EmailViewModel email = new EmailViewModel();
                            email.MailSubject = emailTemplate.EmailSubject;
                            email.MailBody = emailTemplate.EmailTemplate1.Replace(Helper.Constants.PL_ExpertName, campus.CampusName).Replace(Helper.Constants.PL_CallBackUrl, callbackUrl).Replace(Helper.Constants.PL_Twitter, Helper.Constants.TwitterLink).Replace(Helper.Constants.PL_Facebook, Helper.Constants.FacebookLink).Replace(Helper.Constants.PL_WebindLink, Helper.Constants.WebindLink).Replace(Helper.Constants.PL_WebindMail, Helper.Constants.WebinMail);
                            email.MailToList = new List<string>();
                            email.MailBCC = new List<string>();
                            email.MailToList.Add(user.Email);
                            emailHelper.SendMail(email);
                            UserManager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        }
                        catch (Exception e)
                        {
                            return View("Error", "Home");
                        }
                    }
                    else
                    {
                        StudentProfile student = new StudentProfile()
                        {
                            StudentName = model.StudentName,
                            Year = model.Year,
                            LastUpdated = DateTime.Now,
                            CampusName = model.CampusName,
                            DepartmentName = model.DepartmentName,
                            EmailID = model.Email,
                            Password = model.Password,
                            LoginID = model.Email,
                            RegisteredOn = DateTime.Now,
                            IsConfirmed = false,
                            ProfilePicID = 4,
                            ContactNumber = model.ContactNo
                        };
                        _Context.StudentProfiles.Add(student);
                        _Context.SaveChanges();

                        try
                        {
                            EmailTemplate emailTemplate = _Context.EmailTemplates.Find(2);
                            EmailViewModel email = new EmailViewModel();
                            email.MailSubject = emailTemplate.EmailSubject;
                            email.MailBody = emailTemplate.EmailTemplate1.Replace(Helper.Constants.PL_ExpertName, student.CampusName).Replace(Helper.Constants.PL_CallBackUrl, callbackUrl).Replace(Helper.Constants.PL_Twitter, Helper.Constants.TwitterLink).Replace(Helper.Constants.PL_Facebook, Helper.Constants.FacebookLink).Replace(Helper.Constants.PL_WebindLink, Helper.Constants.WebindLink).Replace(Helper.Constants.PL_WebindMail, Helper.Constants.WebinMail);
                            email.MailToList = new List<string>();
                            email.MailBCC = new List<string>();
                            email.MailToList.Add(user.Email);
                            emailHelper.SendMail(email);
                            UserManager.SendEmail(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        }
                        catch (Exception e)
                        {
                            return View("Error", "Home");
                        }
                    }
                    return RedirectToAction("RegisterConfirmation", "Account");
                }
                else
                {
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/RegisterConfirmation
        [AllowAnonymous]
        public ActionResult RegisterConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            IdentityResult result = await UserManager.ConfirmEmailAsync(userId, code);
            if (result.Succeeded)
            {
                var user = UserManager.FindById(userId);
                if (user != null)
                {
                    using (WeBindDemoEntities context = new WeBindDemoEntities())
                    {
                        EmailViewModel email = new EmailViewModel();
                        if (int.Parse(user.Roles.FirstOrDefault().RoleId) == (int)Roles.Expert)
                        {
                            ExpertProfile expert = context.ExpertProfiles.Where(x => x.EmailID == user.Email).FirstOrDefault();
                            expert.IsConfirmed = true;
                            context.Entry(expert).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();

                            EmailTemplate emailTemplate = _Context.EmailTemplates.Find(4);
                            email.MailSubject = emailTemplate.EmailSubject;
                            email.MailBody = emailTemplate.EmailTemplate1.Replace(Helper.Constants.PL_ExpertName, expert.FirstName)
                                .Replace(Helper.Constants.PL_E_DashBoard, Helper.Constants.PL_E_DashBoard_link)
                                .Replace(Helper.Constants.PL_E_Feedback, Helper.Constants.PL_E_Feedback_link)
                                .Replace(Helper.Constants.PL_E_Messages, Helper.Constants.PL_E_Messages_link)
                                .Replace(Helper.Constants.PL_E_Profile, Helper.Constants.PL_E_Profile_link)
                                .Replace(Helper.Constants.PL_E_ReferFriend, Helper.Constants.PL_E_ReferFriend_link)
                                .Replace(Helper.Constants.PL_E_Requests, Helper.Constants.PL_E_Requests_link)
                                .Replace(Helper.Constants.PL_E_Sessions, Helper.Constants.PL_E_Sessions_link)
                                .Replace(Helper.Constants.PL_Twitter, Helper.Constants.TwitterLink).Replace(Helper.Constants.PL_Facebook, Helper.Constants.FacebookLink).Replace(Helper.Constants.PL_WebindLink, Helper.Constants.WebindLink).Replace(Helper.Constants.PL_WebindMail, Helper.Constants.WebinMail);
                            email.MailToList = new List<string>();
                            email.MailBCC = new List<string>();
                            email.MailToList.Add(user.Email);
                            emailHelper.SendMail(email);
                        }
                        else
                        {
                            CampusProfile campus = context.CampusProfiles.Where(x => x.EmailID == user.Email).FirstOrDefault();
                            campus.IsConfirmed = true;
                            context.Entry(campus).State = System.Data.Entity.EntityState.Modified;
                            context.SaveChanges();

                            EmailTemplate emailTemplate = _Context.EmailTemplates.Find(3);
                            email.MailSubject = emailTemplate.EmailSubject;
                            email.MailBody = emailTemplate.EmailTemplate1.Replace(Helper.Constants.PL_ExpertName, campus.CampusName)
                                .Replace(Helper.Constants.PL_LatestSession, Helper.Constants.PL_LatestSession_link)
                                .Replace(Helper.Constants.PL_UpdateProfile, Helper.Constants.PL_UpdateProfile_link)
                                .Replace(Helper.Constants.PL_RequestSession, Helper.Constants.PL_RequestSession_link)
                                .Replace(Helper.Constants.PL_SearchExpert, Helper.Constants.PL_SearchExpert_link)
                                .Replace(Helper.Constants.PL_SendMessage, Helper.Constants.PL_SendMessage_link)
                                .Replace(Helper.Constants.PL_SendFeedback, Helper.Constants.PL_SendFeedback_link)
                                .Replace(Helper.Constants.PL_Twitter, Helper.Constants.TwitterLink).Replace(Helper.Constants.PL_Facebook, Helper.Constants.FacebookLink).Replace(Helper.Constants.PL_WebindLink, Helper.Constants.WebindLink).Replace(Helper.Constants.PL_WebindMail, Helper.Constants.WebinMail);
                            email.MailToList = new List<string>();
                            email.MailBCC = new List<string>();
                            email.MailToList.Add(user.Email);
                            emailHelper.SendMail(email);
                        }
                    }
                }
                return View("ConfirmEmail");
            }
            else
            {
                AddErrors(result);
                return View();
            }
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    ModelState.AddModelError("", "The user either does not exist or is not confirmed.");
                    return View();
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                try
                {
                    EmailViewModel email = new EmailViewModel();
                    email.MailSubject = "WeBind || Reset Password";
                    email.MailBody = "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>";
                    email.MailToList = new List<string>();
                    email.MailBCC = new List<string>();
                    email.MailToList.Add(user.Email);
                    emailHelper.SendMail(email);
                    await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                }
                catch (Exception e)
                {
                    throw;
                }

                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            if (code == null)
            {
                return View("Error");
            }
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("", "No user found.");
                    return View();
                }
                IdentityResult result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("ResetPasswordConfirmation", "Account");
                }
                else
                {
                    AddErrors(result);
                    return View();
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/Disassociate
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
        {
            ManageMessageId? message = null;
            IdentityResult result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                await SignInAsync(user, isPersistent: false);
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("Manage", new { Message = message });
        }

        //
        // GET: /Account/Manage
        public ActionResult Manage(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("Manage");
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Manage(ManageUserViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("Manage");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result.Succeeded)
                    {
                        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                        await SignInAsync(user, isPersistent: false);
                        return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["OldPassword"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
                    }
                    else
                    {
                        AddErrors(result);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
                return RedirectToLocal(returnUrl);
            }
            else
            {
                // If the user does not have an account, then prompt the user to create an account
                ViewBag.ReturnUrl = returnUrl;
                ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
        }

        //
        // GET: /Account/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
            }
            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage");
            }
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInAsync(user, isPersistent: false);

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // SendEmail(user.Email, callbackUrl, "Confirm your account", "Please confirm your account by clicking this link");

                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult RemoveAccountList()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return (ActionResult)PartialView("_RemoveAccountPartial", linkedAccounts);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private void SendEmail(string email, string callbackUrl, string subject, string message)
        {
            // For information on sending mail, please visit http://go.microsoft.com/fwlink/?LinkID=320771
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            Error
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private ActionResult RedirectToMasterDashboard(string returnUrl, long userId)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("ExpertDashBoard", "Expert");
            }
        }

        private ActionResult RedirectToCampusDashboard(string returnUrl, long userId)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("EducatorUpcomingMCList", "Educator");
            }
        }

        private ActionResult RedirectToStudentDashboard(string returnUrl, long userId)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("StudentDashBoard", "Student");
            }
        }


        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}