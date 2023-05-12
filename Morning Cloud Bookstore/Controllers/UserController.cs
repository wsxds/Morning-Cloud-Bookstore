using Morning_Cloud_Bookstore.Models;
using Morning_Cloud_Bookstore.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Morning_Cloud_Bookstore.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetUser() 
        {
            using (DBEntities db = new DBEntities())
            {
                var list = db.Users.Select(u=>new UserModel { 
                    UserID = u.UserID,
                    UserEmail = u.UserEmail,
                    UserName = u.UserName,
                    UserNick = u.UserNick,
                    UserPwd= u.UserPwd,
                    UserSex=u.UserSex,
                }).ToList();
                return Json(list,JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetUserWhere(string name,string nick)
        {
            using (DBEntities db = new DBEntities())
            {
                IQueryable<Users> q = db.Users;
                if (!string.IsNullOrEmpty(name))
                {
                    q = q.Where(u => u.UserName.Contains(name));
                }
                if (!string.IsNullOrEmpty(nick)) 
                {
                    q = q.Where(u => u.UserNick.Contains(nick));
                }
                var list = q.Select(u => new UserModel
                {
                    UserID = u.UserID,
                    UserEmail = u.UserEmail,
                    UserName = u.UserName,
                    UserNick = u.UserNick,
                    UserPwd = u.UserPwd,
                    UserSex = u.UserSex,
                }).ToList();
                return Json(list);
            }
        }
    }
}