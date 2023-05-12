using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Morning_Cloud_Bookstore.Models;
using Morning_Cloud_Bookstore.Models.Model;

namespace Morning_Cloud_Bookstore.Controllers
{
    public class BillingController : Controller
    {
        // GET: Billing
        public ActionResult Orders()
        {
            return View();
        }
        public ActionResult OrderDetail(int? oid)
        {
            Session["oid"] = oid;
            return View();
        }
        public ActionResult OrderDetailView() 
        {
            return View();
        }
        public async Task<JsonResult> GetUserOrder(int? oid) 
        {
            using (DBEntities db = new DBEntities())
            {
                string sql = @"select od.OrderNum,od.OrderDate,od.OrderMoney,od.OrderState,u.UserName,u.UserNick,am.AMTel,am.AMAddress 
                        from AddressManager am join Orders od on am.AMID = od.AMID join Users u on od.UserID = u.UserID where od.OrderID = "+oid;
                OrdersModel order = await db.Database.SqlQuery<OrdersModel>(sql).FirstOrDefaultAsync();
                return Json(order);
            }
        }
        public ActionResult GetOrders()
        {
            using (DBEntities db = new DBEntities())
            {
                var list = db.Orders.Where(o => o.OrderID > 0).Select(o => new OrdersModel
                {
                    OrderNum = o.OrderNum,
                    OrderID = o.OrderID,
                    UserID = o.UserID,
                    UserNick = o.Users.UserNick,
                    OrderDate = o.OrderDate,
                    OrderMoney = o.OrderMoney,
                    OrderState = o.OrderState

                }).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <returns></returns>
        public ActionResult SelOrder(OrdersModel od) 
        {
            using (DBEntities db = new DBEntities())
            {
                var q = db.Orders.Where(o=>o.OrderID>0);
                if (!string.IsNullOrEmpty(od.OrderNum))
                {
                    q = q.Where(o => o.OrderNum == od.OrderNum);
                }
                if (!string.IsNullOrEmpty(od.UserNick))
                {
                    q = q.Where(o => o.Users.UserNick.Contains(od.UserNick));
                }
                if (od.OrderState>0)
                {
                    q = q.Where(o => o.OrderState == od.OrderState);
                }
                var list = q.Select(o => new OrdersModel
                {
                    OrderNum = o.OrderNum,
                    OrderID = o.OrderID,
                    UserID = o.UserID,
                    UserNick = o.Users.UserNick,
                    OrderDate = o.OrderDate,
                    OrderMoney = o.OrderMoney,
                    OrderState = o.OrderState
                }).ToList();
                return Json(list);
            }
        }
        public ActionResult GetOrderDetail(int oid) 
        {
            using (DBEntities db = new DBEntities())
            {
                var od = db.OrderDetail.Where(o => o.OrderID == oid).Select(o => new OrderDetailModel
                {
                    BookTitle = o.Books.BookTitle,
                    ODPrice = o.ODPrice,
                    ODCount = o.ODCount,
                    ODMoney = o.ODMoney
                }).ToList();
                return Json(od);
            }
        }
        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public ActionResult EditState(int id) 
        {
            using (DBEntities db = new DBEntities())
            {
               var q = db.Orders.Where(o => o.OrderID == id).FirstOrDefault();
                if (q.OrderState<4)
                {
                    q.OrderState += 1;
                }
                int count = db.SaveChanges();
                return Json(new { count });
            }
        }
    }
}