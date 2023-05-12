using Morning_Cloud_Bookstore.Models;
using Morning_Cloud_Bookstore.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Morning_Cloud_Bookstore.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            object name = AdminHelper.AdminUser;
            if (name == null)
            {
                return View("../Login/Index");
            }
            return View(name);
        }
    }
}