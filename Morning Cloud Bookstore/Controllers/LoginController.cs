using BLL;
using Morning_Cloud_Bookstore.Models;
using Morning_Cloud_Bookstore.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Morning_Cloud_Bookstore.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login(string name,string pwd) 
        {
            DBEntities db = new DBEntities();
            int count = -1;
            var q = db.Admins.Where(a => a.AdminUser.Equals(name)).FirstOrDefault();
            AdminHelper.AdminUser = q.AdminUser;
            AdminHelper.AdminID = q.AdminID;
            string pasd = MD5Service.GetMD5CodeToString(pwd);
            if (q.AdminPwd.Equals(pasd))
            {
                count= 1;
            }
            return Json( count);
        }
    }
}