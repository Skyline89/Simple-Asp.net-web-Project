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
        public List<LOOKUPAvailableRole> GetAllRoles()
        {
            using (TestDBEntities db = new TestDBEntities())
            {
                var roles = db.LOOKUPRoles.Select(o => new LOOKUPAvailableRole
                {
                    LOOKUPRoleID = o.LOOKUPRoleID,
                    RoleName = o.RoleName,
                    RoleDescription = o.RoleDescription
                }).ToList();

                return roles;
            }
        }
        public int GetUserID(string loginName)
        {
            using (TestDBEntities db = new TestDBEntities()) { 
                var user = db.SYSUsers.Where(x => x.LoginName.Equals(loginName));
                if (user.Any())
                    return user.FirstOrDefault().SYSUserID;
            }
            return 0;
        }
        public List<UserProfileView> GetAllUserProfiles()
        {
            List<UserProfileView> profiles = new List<UserProfileView>();
            using (TestDBEntities db = new TestDBEntities())
            {
                UserProfileView Upv;
                var users = db.SYSUsers.ToList();

                foreach (SYSUser i in db.SYSUsers)
                {
                    Upv = new UserProfileView();
                    Upv.SYSUserID = i.SYSUserID;
                    Upv.LoginName = i.LoginName;
                    Upv.Password = i.PasswordEncryptedText;

                    var Sup = db.SysUserProfiles.Find(i.SYSUserID);
                    if (Sup != null)
                    {
                        Upv.FirstName = Sup.FirstName;
                        Upv.LastName = Sup.LastName;
                        Upv.Gender = Sup.Gender;
                    }
                    var Sur = db.SysUserRoles.Where(x => x.SYSUserID.Equals(i.SYSUserID));
                    if (Sur.Any())
                    {
                        var userRole = Sur.FirstOrDefault();
                        Upv.LOOKUPRoleID = userRole.LOOKUPRoleID;
                        Upv.RoleName = userRole.LOOKUPRole.RoleName;
                        Upv.IsRoleActive = userRole.IsActive;
                    }
                    profiles.Add(Upv);
                }
            }
            return profiles;
        }
        public UserDataView GetUserDataView(string loginName)
        {
            UserDataView Udv = new UserDataView();
            List<UserProfileView> profiles = GetAllUserProfiles();
            List<LOOKUPAvailableRole> roles = GetAllRoles();

            int? userAssignRoleID = 0, userID = 0;
            string userGender = string.Empty;

            userID = GetUserID(loginName);
            using (TestDBEntities db = new TestDBEntities())
            {
                userAssignRoleID = db.SysUserRoles.Where(x => x.SYSUserID == userID)?.FirstOrDefault().LOOKUPRoleID;
                userGender = db.SysUserProfiles.Where(x => x.SYSUserID == userID)?.FirstOrDefault().Gender;
            }
            List<Gender> genders = new List<Gender>();
            genders.Add(new Gender { Text = "Male", Value = "M" });
            genders.Add(new Gender { Text = "Female", Value = "F" });

            Udv.UserProfile = profiles;
            Udv.UserRoles = new UserRoles { SelectedRoleID = userAssignRoleID, UserRoleList = roles };
            Udv.UserGenders = new UserGender { SelectGender = userGender, Gender = genders };
            return Udv;
        }
        public void UpdateUserAccount(UserProfileView user)
        {
            using (TestDBEntities db = new TestDBEntities())
            {
                using (var dbContextTrasaction = db.Database.BeginTransaction())
                { 
                    try {
                        SYSUser Su = db.SYSUsers.Find(user.SYSUserID);
                        Su.LoginName = user.LoginName;
                        Su.PasswordEncryptedText = user.Password;
                        Su.RowCreatedSYSUserID = user.SYSUserID;
                        Su.RowModifiedSYSUserID = user.SYSUserID;
                        Su.RowCreatedDateTime = DateTime.Now;
                        Su.RowModifiedDateTime = DateTime.Now;
                        db.SaveChanges();

                        var userProfile = db.SysUserProfiles.Where(x => x.SYSUserID == user.SYSUserID);
                            if (userProfile.Any())
                            {
                                SysUserProfile Sup = userProfile.FirstOrDefault();
                                Sup.SYSUserID = Su.SYSUserID;
                                Sup.FirstName = user.FirstName;
                                Sup.LastName = user.LastName;
                                Sup.Gender = user.Gender;
                                Sup.RowCreatedSYSUserID = user.SYSUserID;
                                Sup.RowModifiedSYSUserID = user.SYSUserID;
                                Sup.RowCreatedDateTime = DateTime.Now;
                                Sup.RowModifiedDateTime = DateTime.Now;

                                db.SaveChanges();
                            }
                                if (user.LOOKUPRoleID > 0)
                                {
                                    var userRole = db.SysUserRoles.Where(x => x.SYSUserID == user.SYSUserID);
                                    SysUserRole Sur = null;
                                    if (userRole.Any())
                                    {
                                        Sur = userRole.FirstOrDefault();
                                        Sur.LOOKUPRoleID = user.LOOKUPRoleID;
                                        Sur.SYSUserID = user.SYSUserID;
                                        Sur.IsActive = true;
                                        Sur.RowCreatedSYSUserID = user.SYSUserID;
                                        Sur.RowModifiedSYSUserID = user.SYSUserID;
                                        Sur.RowCreatedDateTime = DateTime.Now;
                                        Sur.RowModifiedDateTime = DateTime.Now;
                                    }
                                    else
                                    {
                                            Sur = new SysUserRole();
                                            Sur.LOOKUPRoleID = user.LOOKUPRoleID;
                                            Sur.SYSUserID = user.SYSUserID;
                                            Sur.IsActive = true;
                                            Sur.RowCreatedSYSUserID = user.SYSUserID;
                                            Sur.RowModifiedSYSUserID = user.SYSUserID;
                                            Sur.RowCreatedDateTime = DateTime.Now;
                                            Sur.RowModifiedDateTime = DateTime.Now;
                                            db.SysUserRoles.Add(Sur);
                                    }
                                    db.SaveChanges();
                                }
                        dbContextTrasaction.Commit();
                    }
                    catch
                    {
                        dbContextTrasaction.Rollback();
                    }
                }
            }
        }
        public void DeleteUser (int userID)
        {
            using (TestDBEntities db = new TestDBEntities())
            {
                using (var dbContextTransaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var Sur = db.SysUserRoles.Where(x => x.SYSUserID == userID);
                        if (Sur.Any())
                        {
                            db.SysUserRoles.Remove(Sur.FirstOrDefault());
                            db.SaveChanges();
                        }
                        var Sup = db.SysUserProfiles.Where(x => x.SYSUserID == userID);
                        if (Sup.Any())
                        {
                            db.SysUserProfiles.Remove(Sup.FirstOrDefault());
                            db.SaveChanges();
                        }
                        var Su = db.SYSUsers.Where(x => x.SYSUserID == userID);
                        if (Su.Any())
                        {
                            db.SYSUsers.Remove(Su.FirstOrDefault());
                            db.SaveChanges();
                        }
                        dbContextTransaction.Commit();
                    }
                    catch
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
        }
        public UserProfileView GetUserProfile(int userID)
        {
            UserProfileView Upv = new UserProfileView();
            using(TestDBEntities db = new TestDBEntities())
            {
                var user = db.SYSUsers.Find(userID);
                if (user != null)
                {
                    Upv.SYSUserID = user.SYSUserID;
                    Upv.LoginName = user.LoginName;
                    Upv.Password = user.PasswordEncryptedText;

                    var Sup = db.SysUserProfiles.Find(userID);
                    if (Sup != null)
                    {
                        Upv.FirstName = Sup.FirstName;
                        Upv.LastName = Sup.LastName;
                        Upv.Gender = Sup.Gender;
                    }
                    var Sur = db.SysUserRoles.Find(userID);
                    if(Sur != null) 
                    {
                        Upv.LOOKUPRoleID = Sur.LOOKUPRoleID;
                        Upv.RoleName = Sur.LOOKUPRole.RoleName;
                        Upv.IsRoleActive = Sur.IsActive;
                    }
                } 
            }
            return Upv;
        }
        public List<UserManager> GetAllMessages()
        {
            using (TestDBEntities db = new TestDBEntities())
            {
                var m = (from q in db.SYSUsers
                         join q1 in db.Messages on q.SYSUserID equals q1.SYSUserID
                         join q2 in db.SysUserProfiles on q.SYSUserID equals q2.SYSUserID
                         select new UserMessage
                         {
                             MessageID = q1.MessageID,
                             SYSUserID = q.SYSUserID,
                             FirstName = q2.FirstName,
                             LastName = q2.LastName,
                             MessageText = q1.MessageText,
                             LogDate = q2.DatePosted
                         }).OrderBy(o => o.LogDate);
                return m.Tolist();
            }
        }
        public void AddMessage(int userID, string messageText)
        {
            using (TestDBEntities db = new TestDBEntities())
            {
                Message m = new Message();
                m.MessageText = messageText;
                m.SYSUserID = userID;
                m.DatePosted = DateTime.UtcNow;

                db.Messages.Add(m);
                db.SaveChanges();
            }
        }
        public int GetUserId(string loginName)
        {
            using (TestDBEntities db = new TestDBEntities())
            {
                return db.SYSUsers.Where(o => o.LoginName.Equals(loginName)).SingleOrDefault().SYSUserID;
            }
        }
    }
}