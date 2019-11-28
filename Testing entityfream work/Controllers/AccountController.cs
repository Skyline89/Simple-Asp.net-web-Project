using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Testing_entityfream_work.Models.EntityManager;
using Testing_entityfream_work.Models.ViewModel;

namespace Testing_entityfream_work.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(UserSignUpView Usv)
        {
            if (ModelState.IsValid)
            {
                UserManager Um = new UserManager();
                if (!Um.IsLoginNameExist(Usv.LoginName))
                {
                    Um.AddUserAccount(Usv);
                    FormsAuthentication.SetAuthCookie(Usv.FirstName, false);
                    return RedirectToAction("Welcome", "Home");
                }
                else
                    ModelState.AddModelError("", "Login Name Already Taken");
            }
            return View();
        }
        public ActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(UserLoginView Ulv, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                UserManager UM = new UserManager();
                string password = UM.GetUserPassword(Ulv.LoginName);

                if (string.IsNullOrEmpty(password))
                    ModelState.AddModelError("", "The user login or password is incorrect.");
                else
                {
                    if (Ulv.Password.Equals(password))
                    {
                        FormsAuthentication.SetAuthCookie(Ulv.LoginName, false);
                        return RedirectToAction("Welcome", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "the password is incorrect");
                    }
                }
            }
            return View(Ulv);
        }
        [Authorize]
        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }
    }
}