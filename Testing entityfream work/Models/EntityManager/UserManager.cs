using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Testing_entityfream_work.Models.ViewModel;
using Testing_entityfream_work.Models.DB;

namespace Testing_entityfream_work.Models.EntityManager
{
    public class UserManager
    {
        public void AddUserAccount(UserSignUpView user)
        {
            using (TestDBEntities db = new TestDBEntities ())
            {
                SYSUser Su = new SYSUser();
                Su.LoginName = user.LoginName;
                Su.PasswordEncryptedText = user.Password;
                Su.RowCreatedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                Su.RowModifiedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                Su.RowCreatedDateTime = DateTime.Now;
                Su.RowModifiedDateTime = DateTime.Now;

                db.SYSUsers.Add(Su);
                db.SaveChanges();

                SysUserProfile Sup = new SysUserProfile();
                Sup.SYSUserID = Su.SYSUserID;
                Sup.FirstName = user.FirstName;
                Sup.LastName = user.LastName;
                Sup.Gender = user.Gender;
                Sup.RowCreatedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                Sup.RowModifiedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                Sup.RowCreatedDateTime = DateTime.Now;
                Sup.RowModifiedDateTime = DateTime.Now;

                db.SysUserProfiles.Add(Sup);
                db.SaveChanges();

                if (user.LOOKUPRoleID > 0)
                {
                    SysUserRole Sur = new SysUserRole();
                    Sur.LOOKUPRoleID = user.LOOKUPRoleID;
                    Sur.SYSUserID = user.SYSUserID;
                    Sur.IsActive = true;
                    Sur.RowCreatedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                    Sur.RowModifiedSYSUserID = user.SYSUserID > 0 ? user.SYSUserID : 1;
                    Sup.RowCreatedDateTime = DateTime.Now;
                    Sup.RowModifiedDateTime = DateTime.Now;

                    db.SysUserRoles.Add(Sur);
                    db.SaveChanges();
                }
            }
        }
        public bool IsLoginNameExist(string loginName)
        {
            using (TestDBEntities db = new TestDBEntities())
            {
                return db.SYSUsers.Where(x => x.LoginName.Equals(loginName)).Any();
            }
        }
        public string GetUserPassword(string loginName)
        {
            using(TestDBEntities db = new TestDBEntities())
            {
                var user = db.SYSUsers.Where(x => x.LoginName.ToLower().Equals(loginName));
                if (user.Any())
                    return user.FirstOrDefault().PasswordEncryptedText;
                else
                    return string.Empty;
            }
        }
       public bool IsUserInRole(string loginName, string roleName)
        {
            using (TestDBEntities db = new TestDBEntities())
            {
                SYSUser Su = db.SYSUsers.Where(x => x.LoginName.ToLower().Equals(loginName))?.FirstOrDefault();
                if(Su != null)
                {
                    var roles = from q in db.SysUserRoles
                                join r in db.LOOKUPRoles on q.LOOKUPRoleID equals r.LOOKUPRoleID
                                where r.RoleName.Equals(roleName) && q.SYSUserID.Equals(Su.SYSUserID)
                                select r.RoleName;
                     if (roles != null)
                     {
                         return roles.Any();
                     }
                }
               return false;
            }
        }
    }
}