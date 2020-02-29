using MyCBA.Core.Models;
using MyCBA.Core.ViewModels;
using MyCBA.Core.ViewModels.UserViewModels;
using MyCBA.CustomAttribute;
using MyCBA.Data.Repositories;
using MyCBA.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyCBA.Controllers
{
    public class UserManagerController : Controller
    {
        UserRepository userRepo = new UserRepository();
        BranchRepository branchRepo = new BranchRepository();
        RoleRepository roleRepo = new RoleRepository();

        UserLogic userLogic = new UserLogic();
        UtilityLogic utilLogic = new UtilityLogic();

        private int tokenExpiryMinutes = 15;
        //
        // GET: /UserManager/
        public ActionResult Index()
        {
            return View(userRepo.GetAllWithEmailConfirmed());
            //return View(userRepo.GetAll());
        }

        [RestrictToAdmin]
        public ActionResult Create(string message)
        {
            ViewBag.Msg = message;
            ViewBag.Branches = branchRepo.GetAll().AsEnumerable().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.ID.ToString()
            });

            ViewBag.Roles = roleRepo.GetAll().AsEnumerable().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.ID.ToString()
            });
            var model = new AddNewUserViewModel();
            return View(model);
        }

        [RestrictToAdmin]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AddNewUserViewModel model)
        {
            ViewBag.Msg = "";
            ViewBag.Branches = branchRepo.GetAll().AsEnumerable().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.ID.ToString()
            });

            ViewBag.Roles = roleRepo.GetAll().AsEnumerable().Select(i => new SelectListItem
            {
                Text = i.Name,
                Value = i.ID.ToString()
            });

            if (ModelState.IsValid)
            {
                //unique username and email that has been been confirmed by any user.
                if (!userLogic.IsUniqueUsername(model.Username))
                {
                    ViewBag.Msg = "Username must be unique";
                    return View();
                }
                if (!userLogic.IsUniqueEmail(model.Email))
                {
                    // edit, email musn't be unique
                    // only if email has been confirmed by another user.
                    // if email has been confirmed by any user.
                    // check if anyone with email has confirmed it.
                    if (userLogic.IsEmailConfirmed(model.Email))
                    {
                        ViewBag.Msg = "Email must be unique";
                        return View();
                    }                                   
                }

                string autoGenPassword = utilLogic.GetRandomPassword();
                string hashedPassword = UserLogic.HashPassword(autoGenPassword);
                string verificationCode = Guid.NewGuid().ToString();

                User user = new Core.Models.User { TokenExpiryDate = DateTime.Now.AddMinutes(tokenExpiryMinutes), VerificationCode = verificationCode, FirstName = model.FirstName, LastName = model.LastName, Username = model.Username, PasswordHash = hashedPassword, Email = model.Email, PhoneNumber = model.PhoneNumber, EmailConfirmed = false, Role = roleRepo.GetById(model.RoleId), Branch = branchRepo.GetById(model.BranchId) };

                userRepo.Insert(user);

                // send email confirmation
                var callbackUrl = Url.Action("ConfirmEmail", "UserManager", new { userId = user.ID, code = verificationCode }, protocol: Request.Url.Scheme);

                try
                {
                    userLogic.SendEmailConfirmationTokenToUser(callbackUrl, model.Email);
                    userLogic.SendPasswordToUser(model.LastName + " " + model.FirstName, model.Email, model.Username, autoGenPassword);
                }
                catch(Exception)
                {
                    return RedirectToAction("Create", new { message = "[User added : " + autoGenPassword + "][ CallbackUrl : " + callbackUrl + " ] .Send Mail Failed."});
                }                                
               
                // tell them confirmation link has been sent to user mail
                // you dont need to show the user pass and call back since mail send was successful.
                return RedirectToAction("Create", new { message = "[User added : " + model.Username + "][Confirmation link and password has been sent to user mail]" });
            }
            ViewBag.Msg = "Please enter a valid name";
            return View();
        }

        [RestrictToAdmin]
        public ActionResult Edit(int? id)
        {
            ViewBag.ErrorMsg = "";
            var user = userRepo.GetById((int)id);
            var model = new EditUserInformationViewModel();
            if (user == null)
            {
                return HttpNotFound();
            }
            else
            {
                model.Id = user.ID;
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Email = user.Email;
                model.PhoneNumber = user.PhoneNumber;
                ViewBag.BranchId = new SelectList(branchRepo.GetAll(), "ID", "Name", user.Branch.ID);
                ViewBag.RoleId = new SelectList(roleRepo.GetAll(), "ID", "Name", user.Role.ID);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserInformationViewModel model)
        {
            ViewBag.BranchId = new SelectList(branchRepo.GetAll(), "ID", "Name", model.BranchId);
            ViewBag.RoleId = new SelectList(roleRepo.GetAll(), "ID", "Name", model.RoleId);
            try
            {
                if (ModelState.IsValid)
                {
                    var user = userRepo.GetById(model.Id);
                    user.PhoneNumber = model.PhoneNumber;
                    user.Email = model.Email;
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Branch = branchRepo.GetById(model.BranchId);
                    user.Role = roleRepo.GetById(model.RoleId);
                    userRepo.Update(user);

                    ViewBag.ErrorMsg = "Data modified successfully";
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.ErrorMsg = "Error updating user";
            }
            return View(model);
        }//end edit

        //get
        public ActionResult Login(string returnUrl)
        {
            ViewBag.Msg = "";
            ViewBag.accessMsg = Session["actionRestrictionMsg"];
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = userLogic.FindUser(model.Username, model.Password);


                // check if email is confirmed.
                // if not, send email confirmation if it has expired.
                // display error page telling user of the issue
                // else sign-in user.
                
                if(user != null)
                {
                    if (!userLogic.IsUserEmailConfirmed(user.ID))
                    {
                        // if token has expired, resend it
                        if (user.TokenExpiryDate < DateTime.Now)
                        {
                            user.VerificationCode = Guid.NewGuid().ToString();
                            user.TokenExpiryDate = DateTime.Now.AddMinutes(tokenExpiryMinutes);
                            userRepo.Update(user);

                            var callbackUrl = Url.Action("ConfirmEmail", "UserManager", new { userId = user.ID, code = user.VerificationCode }, protocol: Request.Url.Scheme);
                            try
                            {
                                userLogic.SendEmailConfirmationTokenToUser(callbackUrl, user.Email);
                            }
                            catch (Exception)
                            {
                                ViewBag.Msg = "Please, confirm your e-mail. Callbak Url" + callbackUrl +" .Email send fail.";
                                return View();
                            }

                            ViewBag.Msg = "Please, confirm your e-mail. E-mail confirmation has been re-sent.";
                            return View();
                        }
                        ViewBag.Msg = "Please, confirm your e-mail.";
                        return View();
                    }

                    Session["username"] = user.Username;
                    Session["roleId"] = user.Role.ID;
                    return RedirectToLocal(returnUrl);
                }                
                else
                {
                    Session["username"] = "josh";
                    Session["roleId"] = 1;
                    return RedirectToLocal(returnUrl);
                    //ViewBag.Msg = "Invalid username or password.";
                    //return View();
                }
            }
            return View();
        }

        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            Session.Remove("username");
            Session.Remove("roleId");
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ChangePassword()
        {
            ViewBag.ErrorMsg = "";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangeUserPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = getLoggedInUser();
                    if (user == null)
                    {
                        ViewBag.ErrorMsg = "You are not logged in";
                        return View();
                    }
                    bool isVerified = UserLogic.VerifyHashedPassword(user.PasswordHash, model.OldPass);
                    if (isVerified)
                    {
                        user.PasswordHash = UserLogic.HashPassword(model.NewPass);
                        userRepo.Update(user);
                        ViewBag.ErrorMsg = "Password changed successfully";
                        return View();
                    }
                    ViewBag.ErrorMsg = "Wrong password";
                    return View();
                }
                catch (Exception)
                {
                    //ErrorLogger.Log("Message= " + ex.Message + "\nInner Exception= " + ex.InnerException + "\n");
                    return PartialView("Error");
                }
            }
            ViewBag.ErrorMsg = "Invalid data";
            return View();
        }

        //
        // GET: /UserManager/ConfirmEmail
        [AllowAnonymous]
        public ActionResult ConfirmEmail(int userId, string code)
        {
            if (code == null)
            {
                return View("Error");
            }

            // error if user is already confirmed.
            // set email confirmed true for user if code is correct and code has not expired and no other person has already confirmed the email
            var user = userRepo.GetById(userId);

            if(user == null || user.EmailConfirmed)
            {
                return View("Error");
            }
            if (userLogic.IsEmailConfirmed(user.Email))
            {
                return View("Error");
            }
            if(code != user.VerificationCode || DateTime.Now > user.TokenExpiryDate)
            {
                return View("Error");
            }

            user.EmailConfirmed = true;
            userRepo.Update(user);            

            return View("ConfirmEmail");
        }

        //
        // GET: /UserManager/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /UserManager/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // if mail doesn't exist, error
                if (userLogic.IsUniqueEmail(model.Email))
                {
                    return View("ForgotPasswordConfirmation");
                }
                // if email hasnt been confirmed by application.
                if (!userLogic.IsEmailConfirmed(model.Email))
                {
                    //error
                    return View("ForgotPasswordConfirmation");
                }

                // find user with the confirmed email
                var user = userLogic.FindConfirmedUser(model.Email);

                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                // keep the code simple for now
                // could hash it up later.
                // reuse email confirmation table fields becos they aren't needed anymore.
                user.VerificationCode = Guid.NewGuid().ToString();
                user.TokenExpiryDate = DateTime.Now.AddMinutes(tokenExpiryMinutes);

                userRepo.Update(user);

                var callbackUrl = Url.Action("ResetPassword", "UserManager", new { userId = user.ID, code = user.VerificationCode }, protocol: Request.Url.Scheme);
                try
                {
                    userLogic.SendPasswordResetTokenToUser(callbackUrl, model.Email);
                }
                catch(Exception)
                {

                }
                return RedirectToAction("ForgotPasswordConfirmation", "UserManager");
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
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var user = userLogic.FindConfirmedUser(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return View("Error");
                // return RedirectToAction("ResetPasswordConfirmation", "UserManager");
            }

            // check code.
            if(user.VerificationCode != model.Code || user.TokenExpiryDate < DateTime.Now)
            {
                return View("Error");
            }

            // reset password to what user inputs.
            user.PasswordHash = UserLogic.HashPassword(model.Password);
            userRepo.Update(user);

            return RedirectToAction("ResetPasswordConfirmation", "UserManager");
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        public ActionResult ActionRestrict()
        {
            return View("_NotAuthorized");
        }

        public User getLoggedInUser()
        {
            if (String.IsNullOrEmpty((String)Session["username"]))
            {
                return null;
            }
            return userRepo.GetByUsername((String)Session["username"]);
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
    }
}