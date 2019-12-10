using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Testing_entityfream_work.Models.EntityManager;
using Testing_entityfream_work.Models.ViewModel;
using Testing_entityfream_work.Security;

namespace Testing_entityfream_work.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult Welcome()
        {
            return View();
        }
        [AuthorizeRoles("Admin")]
        public ActionResult AdminOnly()
        {
            return View();
        }
        public ActionResult UnAuthorized()
        {
            return View();
        }
        [AuthorizeRoles("Admin")]
        public ActionResult ManagerUserPartial(string status = "")
        {
            if (User.Identity.IsAuthenticated)
            {
                string loginName = User.Identity.Name;
                UserManager Um = new UserManager();
                UserDataView Udv = Um.GetUserDataView(loginName);
                string message = string.Empty;
                if (status.Equals("update"))
                    message = "Update Successful";
                else if (status.Equals("delete"))
                    message = "Delete Successful";
                ViewBag.Mesaage = message;
                return PartialView(Udv);
            }
            return RedirectToAction("Index", "Home");
        }
        [AuthorizeRoles("Admin")]
        public ActionResult UpdateUserData(int userID, string loginName, string password, string firstname, string lastname, string gender, int roleID = 0)
        {
            UserProfileView Upv = new UserProfileView();
            Upv.SYSUserID = userID;
            Upv.LoginName = loginName;
            Upv.Password = password;
            Upv.FirstName = firstname;
            Upv.LastName = lastname;
            Upv.Gender = gender;
            if (roleID > 0)
                Upv.LOOKUPRoleID = roleID;
            UserManager Um = new UserManager();
            Um.UpdateUserAccount(Upv);
            return Json(new { success = true });
        }
        [AuthorizeRoles("Admin")]
        public ActionResult DeleteUser(int userID)
        {
            UserManager Um = new UserManager();
            Um.DeleteUser(userID);
            return Json(new { success = true });
        }
        [Authorize]
        public ActionResult EditProfile()
        {
            string loginName = User.Identity.Name;
            UserManager Um = new UserManager();
            UserProfileView Upv = Um.GetUserProfile(Um.GetUserID(loginName));
            return View(Upv);
        }
        [HttpPost]
        [Authorize]
        public ActionResult EditProfile(UserProfileView profile)
        {
            if (ModelState.IsValid)
            {
                UserManager Um = new UserManager();
                Um.UpdateUserAccount(profile);

                ViewBag.Status = "Update Sucessfull";
            }
            return View(profile);
        }
    }   
}
