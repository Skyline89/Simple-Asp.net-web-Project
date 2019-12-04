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
        public ActionResult ManagerUserPartial()
        {
            if (User.Identity.IsAuthenticated)
            {
                string loginName = User.Identity.Name;
                UserManager Um = new UserManager();
                UserDataView Udv = Um.GetUserDataView(loginName);
                return PartialView(Udv);
            }
            return View();
        }
    }     
}