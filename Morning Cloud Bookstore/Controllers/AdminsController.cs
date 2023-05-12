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
    public class AdminsController : Controller
    {
        // GET: Admins
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GrZx() 
        {
            return View();
        }
        public ActionResult GetAdmin() 
        {
            using (DBEntities db = new DBEntities())
            {
                var r = db.Admins.ToList();
                return Json(r,JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetAdminByName(string name)
        {
            using (DBEntities db = new DBEntities())
            {
                var r = db.Admins.Where(a=>a.AdminUser.Contains(name)).ToList();
                return Json(r);
            }
        }
        public ActionResult GetAdminsById() 
        {
            int id = AdminHelper.AdminID;
            using (DBEntities db = new DBEntities())
            {
                var admin = db.Admins.Where(a => a.AdminID == id).FirstOrDefault();
                return Json(admin);
            }
        }

        public ActionResult Getoldpwd(string pwd) 
        {
            int id = AdminHelper.AdminID;
            string oldpwd = MD5Service.GetMD5CodeToString(pwd);
            using (DBEntities db = new DBEntities())
            {
                bool flg = false;
                var admin = db.Admins.Where(a => a.AdminID == id).FirstOrDefault();
                if (admin.AdminPwd.Equals(oldpwd))
                {
                    flg= true;
                }
                return Json(flg);
            }
        }
        public ActionResult Editpwd(string pwd) 
        {
            int id = AdminHelper.AdminID;
            string okpwd = MD5Service.GetMD5CodeToString(pwd);
            using (DBEntities db = new DBEntities())
            {
                var admin = db.Admins.Where(a => a.AdminID == id).FirstOrDefault();
                admin.AdminPwd = okpwd;
                var count = db.SaveChanges();
                return Json(count);
            }
        }
    }
}