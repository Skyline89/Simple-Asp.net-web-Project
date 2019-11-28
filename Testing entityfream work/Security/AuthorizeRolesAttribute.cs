using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Testing_entityfream_work.Models.DB;
using Testing_entityfream_work.Models.EntityManager;

namespace Testing_entityfream_work.Security
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        private readonly string[] userAssignRoles;
        public AuthorizeRolesAttribute(params string[] roles)
        {
            this.userAssignRoles = roles;
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            bool authorize = false;
            using (TestDBEntities db = new TestDBEntities())
            {
                UserManager Um = new UserManager();
                foreach( var roles in userAssignRoles)
                {
                    authorize = Um.IsUserInRole(httpContext.User.Identity.Name, roles);
                    if (authorize)
                        return authorize;
                }
            }
           return authorize;
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            filterContext.Result = new RedirectResult("~/Home/UnAuthorized");
        }
    }
}