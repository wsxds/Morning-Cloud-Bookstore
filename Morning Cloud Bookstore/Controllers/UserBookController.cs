using BLL;
using Morning_Cloud_Bookstore.Models;
using Morning_Cloud_Bookstore.Models.Model;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Morning_Cloud_Bookstore.Controllers
{
    /// <summary>
    /// 前台控制器
    /// </summary>
    public class UserBookController : Controller
    {

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        // GET: UserBook
        public ActionResult Index()
        {
            List<BLCategoryModel> BLlist = GetBLCategoryAll();
            Session["Category"] = BLlist;

            List<BooksModel> booklist = GetBookBestSeller();
            Session["Book"] = booklist;

            List<BooksModel> Buylist = GetBookBuy();
            Session["Buylist"] = Buylist;

            List<BooksModel> Hotlist = GetBookHot();
            Session["Hotlist"] = Hotlist;
            return View();
        }
        /// <summary>
        /// 登录视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            List<BLCategoryModel> BLlist = GetBLCategoryAll();
            Session["Category"] = BLlist;
            return View();
        }
        /// <summary>
        /// 地址管理
        /// </summary>
        /// <returns></returns>
        public ActionResult AddressPage()
        {
            List<BLCategoryModel> BLlist = GetBLCategoryAll();
            Session["Category"] = BLlist;

            using (DBEntities db = new DBEntities())
            {
                int UserId = UserIdHelper.ID;
                var addresslist = db.AddressManager.Where(u => u.UserID == UserId).ToList();
                Session["addresslist"] = addresslist;
            }
            return View();
        }
        /// <summary>
        /// 结算
        /// </summary>
        /// <returns></returns>
        public ActionResult DisplayPage()
        {
            List<BLCategoryModel> BLlist = GetBLCategoryAll();
            Session["Category"] = BLlist;
            using (DBEntities db = new DBEntities())
            {
                //获取字典集合
                Dictionary<int, int> dic = Session["dic"] as Dictionary<int, int>;
                int amid = (int)Session["amid"];
                if (amid > 0)
                {
                    Orders o = new Orders();
                    o.AMID = amid;
                    o.UserID = UserIdHelper.ID;
                    o.OrderMoney = (decimal)Session["bookMoneyTotal"];
                    o.OrderState = 1;
                    o.OrderDate = DateTime.Now;
                    string orderNum = "DD" + DateTime.Now.ToString("yyyy-MM-dd").Replace("-", "") + "0001";

                    List<OrderDetail> listDetail = new List<OrderDetail>();
                    //遍历购物车集合，一条数据就是一个订单详情对象
                    foreach (var item in dic.Keys)
                    {
                        OrderDetail od = new OrderDetail();
                        od.BookID = item;
                        od.ODCount = dic[item];
                        //根据书籍的编号获取书籍的售价
                        od.ODPrice = db.Books.Where(b=>b.BookID == od.BookID).FirstOrDefault().BookMoney;
                        od.ODMoney = od.ODPrice * od.ODCount;
                        listDetail.Add(od);
                    }

                    //新增操作
                    string number = orderNum.Substring(0, 10);
                    string orderMaxNum = null;
                    var q = db.Orders.Where(c => c.OrderNum.Contains(number)).OrderByDescending(b => b.OrderID).FirstOrDefault();
                    if (q!=null)
                    {
                        orderMaxNum = q.OrderNum;
                    }
                    //今天产生过订单
                    if (orderMaxNum != null)
                    {
                        int newOrderNum = int.Parse(orderMaxNum.ToString().Substring(10)) + 1;
                        //今天的订单是个位数
                        if (newOrderNum.ToString().Length == 1)
                        {
                            orderNum = orderNum.Substring(0, 13) + newOrderNum;
                        }
                        //今天的订单是十位位数
                        if (newOrderNum.ToString().Length == 2)
                        {
                            orderNum = orderNum.Substring(0, 12) + newOrderNum;
                        }
                        //今天的订单是百位数
                        if (newOrderNum.ToString().Length == 3)
                        {
                            orderNum = orderNum.Substring(0, 11) + newOrderNum;
                        }
                        //今天的订单是千位数
                        if (newOrderNum.ToString().Length == 4)
                        {
                            orderNum = orderNum.Substring(0, 10) + newOrderNum;
                        }
                    }
                    o.OrderNum = orderNum;

                    //成功之后返回当前订单号
                    var order = db.Orders.Add(o);
                    int orderid = order.OrderID;
                    //新增订单详情
                    foreach (var item in listDetail)
                    {
                        item.OrderID = orderid;
                        db.OrderDetail.Add(item);
                    }

                    //书籍数量的更新，库存和销售数量
                    foreach (var item in dic.Keys)
                    {
                        var book = db.Books.Where(bk => bk.BookID == item).FirstOrDefault();
                        book.BookSale += dic[item];
                        book.BookDeport -= dic[item];
                    }
                    db.SaveChanges();
                    Session["num"] = orderNum;
                    Session["dic"] = null;
                }
                else
                {
                    return RedirectToAction("AddressPage");
                }
                return View();
            }
        }
        /// <summary>
        /// 结算界面
        /// </summary>
        /// <returns></returns>
        public ActionResult BalancePage()
        {
            List<BLCategoryModel> BLlist = GetBLCategoryAll();
            Session["Category"] = BLlist;

            using (DBEntities db = new DBEntities())
            {
                //获取地址
                int UserId = UserIdHelper.ID;
                var address = db.AddressManager.Where(a => a.UserID == UserId && a.AMMark == true).FirstOrDefault();
                Session["address"] = address;
                Session["amid"] = address.AMID;


                List<BooksModel> list = new List<BooksModel>();
                if (Session["dic"] != null)
                {
                    //通过session获取字典集合
                    Dictionary<int, int> dic = Session["dic"] as Dictionary<int, int>;
                    string ids = "";
                    if (dic != null)
                    {
                        foreach (var item in dic.Keys)
                        {
                            ids += item + ",";
                        }
                        ids = ids.Remove(ids.Length - 1);
                    }
                    else
                    {
                        ids = "''";
                    }
                    string sql = @"select * from Books where BookID in(" + ids + ")";
                    list = db.Database.SqlQuery<BooksModel>(sql).ToList();
                    //折扣价
                    decimal? bookMoneyTotal = 0;
                    //遍历集合
                    foreach (var item in list)
                    {
                        //计算折扣价
                        bookMoneyTotal += item.BookMoney * dic[item.BookID];
                        Session["bookMoneyTotal"] = bookMoneyTotal;

                        item.XiaoJi = item.BookMoney * dic[item.BookID];
                        item.Count = dic[item.BookID];
                    }
                    Session["list"] = list;


                }

                return View();
            }
        }
        /// <summary>
        /// 注册视图
        /// </summary>
        /// <returns></returns>
        public ActionResult Region()
        {
            List<BLCategoryModel> BLlist = GetBLCategoryAll();
            Session["Category"] = BLlist;
            return View();
        }
        /// <summary>
        /// 注册用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public ActionResult AddUser(Users user)
        {
            using (DBEntities db = new DBEntities())
            {
                int count = 0;
                var q = db.Users.Where(u => u.UserName == user.UserName).FirstOrDefault();
                if (q != null)
                {
                    count = -1;
                }
                else
                {
                    user.UserPwd = MD5Service.GetMD5CodeToString(user.UserPwd);
                    db.Users.Add(user);
                    count = db.SaveChanges();
                }
                return Json(count);
            }
        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="pwd">密码</param>
        /// <returns></returns>
        public ActionResult YanZ(string name, string pwd)
        {
            using (DBEntities db = new DBEntities())
            {
                int count = 0;
                var user = db.Users.Where(u => u.UserName == name).FirstOrDefault();
                if (user != null)
                {
                    string md5pwd = MD5Service.GetMD5CodeToString(pwd);
                    if (user.UserPwd == md5pwd)
                    {
                        count = 1;
                        Session["user"] = user;
                        UserIdHelper.ID = user.UserID;
                    }
                    else
                    {
                        count = -1;
                    }
                }
                return Json(count);
            }
        }
        /// <summary>
        /// 注销登录
        /// </summary>
        /// <returns></returns>
        public ActionResult ZhuXiao()
        {
            Session["user"] = null;
            return RedirectToAction("Login");
        }
        //书籍详细信息页
        public ActionResult BookPage(int id)
        {
            using (DBEntities db = new DBEntities())
            {
                var book = db.Books.Where(c => c.BookIsDelete == false && c.BookID == id).Select(c => new BooksModel
                {
                    BookID = c.BookID,
                    BookTitle = c.BookTitle,
                    BSID = c.BSCategory.BSID,
                    BSName = c.BSCategory.BSName,
                    BLName = c.BSCategory.BLCategory.BLName,
                    BookPublish = c.BookPublish,
                    BookAuthor = c.BookAuthor,
                    BookContent = c.BookContent.Substring(0, 200),
                    BookMoney = c.BookMoney,
                    BookPrice = c.BookPrice,
                    BookCount = c.BookCount,
                    BookDeport = c.BookDeport,
                    BookSale = c.BookSale,
                    BookComm = c.BookComm.Substring(0, 200),
                    BookAuthorDesc = c.BookAuthorDesc.Substring(0, 200),
                    ISBN = c.ISBN,
                    BookDesc = c.BookDesc.Substring(0, 200),
                    ImgUrl = "/BookImages/" + c.ISBN + ".jpg"
                }).FirstOrDefault();


                return View(book);
            }
        }
        public ActionResult flowPage()
        {
            if (Session["user"] != null)
            {
                using (DBEntities db = new DBEntities())
                {
                    List<BooksModel> list = new List<BooksModel>();
                    if (Session["dic"] != null)
                    {
                        //通过session获取字典集合
                        Dictionary<int, int> dic = Session["dic"] as Dictionary<int, int>;
                        string ids = "";
                        if (dic != null)
                        {
                            foreach (var item in dic.Keys)
                            {
                                ids += item + ",";
                            }
                            ids = ids.Remove(ids.Length - 1);
                        }
                        else
                        {
                            ids = "''";
                        }
                        string sql = @"select * from Books where BookID in(" + ids + ")";
                        list = db.Database.SqlQuery<BooksModel>(sql).ToList();
                        //售价
                        decimal? bookPriceTotal = 0;
                        //折扣价
                        decimal? bookMoneyTotal = 0;
                        //遍历集合
                        foreach (var item in list)
                        {
                            //计算售价
                            bookPriceTotal += item.BookPrice * dic[item.BookID];
                            Session["bookPriceTotal"] = bookPriceTotal;
                            //计算折扣价
                            bookMoneyTotal += item.BookMoney * dic[item.BookID];
                            Session["bookMoneyTotal"] = bookMoneyTotal;
                            Session["Yh"] = bookPriceTotal - bookMoneyTotal;

                            item.XiaoJi = item.BookMoney * dic[item.BookID];
                            item.Count = dic[item.BookID];
                        }
                        Session["list"] = list;
                    }

                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
        //类别
        public List<BLCategoryModel> GetBLCategoryAll()
        {
            using (DBEntities db = new DBEntities())
            {
                var blCategory = db.BLCategory.Select(bl => new BLCategoryModel
                {
                    BLID = bl.BLID,
                    BLName = bl.BLName
                }).ToList();
                for (int i = 0; i < blCategory.Count(); i++)
                {
                    int blid = blCategory[i].BLID;
                    blCategory[i].bslist = db.BSCategory.Where(bs => bs.BLID == blid).Select(bs => new BSCategoryModel
                    {
                        BSID = bs.BSID,
                        BSName = bs.BSName
                    }).ToList();
                }

                return blCategory;
            }
        }
        /// <summary>
        /// 秒杀
        /// </summary>
        /// <returns></returns>
        public List<BooksModel> GetBookBuy()
        {
            using (DBEntities db = new DBEntities())
            {
                var book = db.Books.Where(b => b.BookIsDelete == false && b.BookIsBuy == true).OrderByDescending(b => b.BookID).Skip(0).Take(5).Select(b => new BooksModel
                {
                    BookID = b.BookID,
                    ISBN = b.ISBN,
                    BookMoney = b.BookMoney,
                    BookPrice = b.BookPrice,
                    BookTitle = b.BookTitle
                }).ToList();
                for (int i = 0; i < book.Count(); i++)
                {
                    if (book[i].BookTitle.Length > 7)
                    {
                        book[i].BookTitle = book[i].BookTitle.Substring(0, 7) + "....";
                    }
                    else
                    {
                        book[i].BookTitle = book[i].BookTitle;
                    }
                    book[i].ImgUrl = "/BookImages/" + book[i].ISBN + ".jpg";
                }
                return book;
            }
        }
        /// <summary>
        /// 热门
        /// </summary>
        /// <returns></returns>
        public List<BooksModel> GetBookHot()
        {
            using (DBEntities db = new DBEntities())
            {
                var book = db.Books.Where(b => b.BookIsDelete == false && b.BookIsHot == true).OrderByDescending(b => b.BookID).Skip(0).Take(10).Select(b => new BooksModel
                {
                    BookID = b.BookID,
                    ISBN = b.ISBN,
                    BookMoney = b.BookMoney,
                    BookPrice = b.BookPrice,
                    BookTitle = b.BookTitle
                }).ToList();
                for (int i = 0; i < book.Count(); i++)
                {
                    if (book[i].BookTitle.Length > 7)
                    {
                        book[i].BookTitle = book[i].BookTitle.Substring(0, 7) + "....";
                    }
                    else
                    {
                        book[i].BookTitle = book[i].BookTitle;
                    }
                    book[i].ImgUrl = "/BookImages/" + book[i].ISBN + ".jpg";
                }
                return book;
            }
        }
        //畅销
        public List<BooksModel> GetBookBestSeller()
        {
            using (DBEntities db = new DBEntities())
            {
                int id = 1;
                var book = db.Books.Where(b => b.BookIsDelete == false).OrderByDescending(b => b.BookSale).Skip(0).Take(5).Select(b => new BooksModel
                {
                    BookID = b.BookID,
                    ISBN = b.ISBN,
                    BookMoney = b.BookMoney,
                    BookPrice = b.BookPrice,
                    BookTitle = b.BookTitle
                }).ToList();
                for (int i = 0; i < book.Count(); i++)
                {
                    if (book[i].BookTitle.Length > 7)
                    {
                        book[i].BookTitle = book[i].BookTitle.Substring(0, 7) + "....";
                    }
                    else
                    {
                        book[i].BookTitle = book[i].BookTitle;
                    }
                    book[i].ID = id;
                    book[i].TopUrl = "/images/Top_" + book[i].ID + ".gif";
                    book[i].ImgUrl = "/BookImages/" + book[i].ISBN + ".jpg";
                    id++;
                }

                return book;
            }
        }

        //
        /// <summary>
        /// 大类别id查询书籍信息
        /// </summary>
        /// <param name="blid">大类别编号</param>
        /// <param name="page">当前页</param>
        /// <returns></returns>
        public ActionResult BookPageByBLid(int blid,int page)
        {
            Session["blid"] = blid;
            Session["page"] = page;
            using (DBEntities db = new DBEntities())
            {
                var q = db.Books.Where(c => c.BookIsDelete == false && c.BSCategory.BLID == blid);
                
                var count = q.ToList().Count();
                if (count != 0)
                {
                    if (count % 3 == 0)
                    {
                        Session["Weiye"] = count / 3;
                    }
                    else
                    {
                        Session["Weiye"] = count / 3 + 1;
                    }
                }
                else
                {
                    Session["Weiye"] = 1;
                }
                var book = q.OrderBy(c => c.BookID).Skip((page-1)*3).Take(3).Select(c => new BooksModel
                {
                    BookID = c.BookID,
                    BookTitle = c.BookTitle,
                    BSID = c.BSCategory.BSID,
                    BSName = c.BSCategory.BSName,
                    BLName = c.BSCategory.BLCategory.BLName,
                    BookPublish = c.BookPublish,
                    BookAuthor = c.BookAuthor,
                    BookContent = c.BookContent.Substring(0, 200),
                    BookMoney = c.BookMoney,
                    BookPrice = c.BookPrice,
                    BookCount = c.BookCount,
                    BookDeport = c.BookDeport,
                    BookSale = c.BookSale,
                    BookComm = c.BookComm.Substring(0, 200),
                    BookAuthorDesc = c.BookAuthorDesc.Substring(0, 200),
                    ISBN = c.ISBN,
                    BookDesc = c.BookDesc.Substring(0, 200),
                    ImgUrl = "/BookImages/" + c.ISBN + ".jpg"
                }).ToList();
                return View(book);
            }
        }
        //小类别id查询书籍信息
        public ActionResult BookPageByBSid(int bsid,int page)
        {
            Session["bsid"] = bsid;
            Session["page"] = page;
            using (DBEntities db = new DBEntities())
            {
                var q = db.Books.Where(c => c.BookIsDelete == false && c.BSID == bsid);
                var count = q.ToList().Count();
                if (count != 0)
                {
                    if (count % 3 == 0)
                    {
                        Session["Weiye"] = count / 3;
                    }
                    else
                    {
                        Session["Weiye"] = count / 3 + 1;
                    }
                }
                else {
                    Session["Weiye"] = 1;
                }
                var book = q.OrderBy(c => c.BookID).Skip((page-1)*3).Take(3).Select(c => new BooksModel
                {
                    BookID = c.BookID,
                    BookTitle = c.BookTitle,
                    BSID = c.BSCategory.BSID,
                    BSName = c.BSCategory.BSName,
                    BLName = c.BSCategory.BLCategory.BLName,
                    BookPublish = c.BookPublish,
                    BookAuthor = c.BookAuthor,
                    BookContent = c.BookContent.Substring(0, 200),
                    BookMoney = c.BookMoney,
                    BookPrice = c.BookPrice,
                    BookCount = c.BookCount,
                    BookDeport = c.BookDeport,
                    BookSale = c.BookSale,
                    BookComm = c.BookComm.Substring(0, 200),
                    BookAuthorDesc = c.BookAuthorDesc.Substring(0, 200),
                    ISBN = c.ISBN,
                    BookDesc = c.BookDesc.Substring(0, 200),
                    ImgUrl = "/BookImages/" + c.ISBN + ".jpg"
                }).ToList();
                return View(book);
            }
        }
        public ActionResult SelectPage(string name,int page)
        {
            Session["name"] = name;
            Session["page"] = page;
            using (DBEntities db = new DBEntities())
            {
                var q = db.Books.Where(c => c.BookIsDelete == false);
                if (!string.IsNullOrEmpty(name))
                {
                    q = q.Where(c => c.BookTitle.Contains(name));

                    var count = q.ToList().Count();
                    if (count != 0)
                    {
                        if (count % 3 == 0)
                        {
                            Session["Weiye"] = count / 3;
                        }
                        else
                        {
                            Session["Weiye"] = count / 3 + 1;
                        }
                    }
                    else
                    {
                        Session["Weiye"] = 1;
                    }
                }
                var book = q.OrderBy(c => c.BookID).Skip((page - 1) * 3).Take(3).Select(c => new BooksModel
                {
                    BookID = c.BookID,
                    BookTitle = c.BookTitle,
                    BSID = c.BSCategory.BSID,
                    BSName = c.BSCategory.BSName,
                    BLName = c.BSCategory.BLCategory.BLName,
                    BookPublish = c.BookPublish,
                    BookAuthor = c.BookAuthor,
                    BookContent = c.BookContent.Substring(0, 200),
                    BookMoney = c.BookMoney,
                    BookPrice = c.BookPrice,
                    BookCount = c.BookCount,
                    BookDeport = c.BookDeport,
                    BookSale = c.BookSale,
                    BookComm = c.BookComm.Substring(0, 200),
                    BookAuthorDesc = c.BookAuthorDesc.Substring(0, 200),
                    ISBN = c.ISBN,
                    BookDesc = c.BookDesc.Substring(0, 200),
                    ImgUrl = "/BookImages/" + c.ISBN + ".jpg"
                }).ToList();
                return View(book);
            }
        }


        //添加到购物车
        public ActionResult GouWuChe(int id)
        {
            //创建字典集合来作为购物车的数据
            Dictionary<int, int> dic = new Dictionary<int, int>();
            //判断session是否为空，为空就添加，不为空就值加1
            if (Session["dic"] != null)
            {
                dic = Session["dic"] as Dictionary<int, int>;
            }
            if (dic.ContainsKey(id))
            {
                dic[id] += 1;
            }
            else
            {
                dic.Add(id, 1);
            }
            //将字典集合保存到Session中
            Session["dic"] = dic;
            return View("index");
        }
        //数量加一
        public ActionResult AddCount(int id)
        {
            //通过session获取字典集合
            Dictionary<int, int> dic = Session["dic"] as Dictionary<int, int>;
            dic[id] += 1;
            return RedirectToAction("flowPage");
        }
        //数量减一
        public ActionResult JianCount(int id)
        {
            //通过session获取字典集合
            Dictionary<int, int> dic = Session["dic"] as Dictionary<int, int>;
            if (dic[id] > 1)
            {
                dic[id] -= 1;
            }
            return RedirectToAction("flowPage");
        }
        public ActionResult Del(int id)
        {
            //通过session获取字典集合
            Dictionary<int, int> dic = Session["dic"] as Dictionary<int, int>;
            if (dic.Count <= 1)
            {
                Session["dic"] = null;
            }
            else
            {
                dic.Remove(id);

            }
            return RedirectToAction("flowPage");
        }
        /// <summary>
        /// 新增地址
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public ActionResult AddAddress(AddressManager address)
        {
            int userid = UserIdHelper.ID;
            address.UserID = userid;
            address.AMMark = false;
            using (DBEntities db = new DBEntities())
            {
                db.AddressManager.Add(address);
                var count = db.SaveChanges();

                return Json(count);
            }
        }
        /// <summary>
        /// 设置默认地址
        /// </summary>
        /// <returns></returns>
        public ActionResult MoRenAddress(int id)
        {
            using (DBEntities db = new DBEntities())
            {
                int userid = UserIdHelper.ID;
                var address = db.AddressManager.Where(a => a.UserID == userid).ToList();
                foreach (var item in address)
                {
                    if (item.AMID == id)
                    {
                        item.AMMark = true;
                    }
                    else
                    {
                        item.AMMark = false;
                    }
                }
                db.SaveChanges();
            }
            return RedirectToAction("AddressPage");
        }
        /// <summary>
        /// 删除地址
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DelAddress(int id)
        {
            using (DBEntities db = new DBEntities())
            {
                var address = db.AddressManager.Where(a => a.AMID == id).FirstOrDefault();
                db.AddressManager.Remove(address);
                db.SaveChanges();
            }
            return RedirectToAction("AddressPage");
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldpwd">旧密码</param>
        /// <param name="okpwd">新密码</param>
        /// <returns></returns>
        public ActionResult UserPage() 
        {
            return View();
        }
        public ActionResult Editpwd(string oldpwd, string okpwd) 
        {
            int id = UserIdHelper.ID;
            int count = 0;
            using (DBEntities db = new DBEntities())
            {
                var user = db.Users.Where(u => u.UserID == id).FirstOrDefault();
                if (user.UserPwd == MD5Service.GetMD5CodeToString(oldpwd))
                {
                    user.UserPwd = MD5Service.GetMD5CodeToString(okpwd);
                    count = db.SaveChanges();
                }
                else
                {
                    count = -1;
                }
                return Json(count);
            }
        }
        public ActionResult AddGWC(int id) 
        {
            int count = 0;
            //创建字典集合来作为购物车的数据
            Dictionary<int, int> dic = new Dictionary<int, int>();
            //判断session是否为空，为空就添加，不为空就值加1
            if (Session["dic"] != null)
            {
                dic = Session["dic"] as Dictionary<int, int>;
            }
            if (dic.ContainsKey(id))
            {
                dic[id] += 1;
                count = 1;
            }
            else
            {
                dic.Add(id, 1);
                count = 1;
            }
            //将字典集合保存到Session中
            Session["dic"] = dic;
            return Json(count);
        }
    }
}