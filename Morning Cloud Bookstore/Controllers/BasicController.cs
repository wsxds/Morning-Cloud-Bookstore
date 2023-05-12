using Microsoft.Ajax.Utilities;
using Morning_Cloud_Bookstore.Models;
using Morning_Cloud_Bookstore.Models.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.EntitySql;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Morning_Cloud_Bookstore.Controllers
{
    public class BasicController : Controller
    {
        // GET: Basic
        #region 视图
        public ActionResult Book()
        {
            return View();
        }

        public ActionResult BookBuy()
        {
            return View();
        }

        public ActionResult BookHot()
        {
            return View();
        }
        public ActionResult BookDel()
        {
            return View();
        }
        public ActionResult AddBookPage()
        {
            return View();
        }
        #endregion

        #region 基本书籍

        /// <summary>
        /// 普通书籍
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBooks()
        {
            using (DBEntities db = new DBEntities())
            {
                var list = db.Books.Where(c => c.BookIsDelete == false && c.BookIsHot == false && c.BookIsBuy == false).Select(c => new BooksModel
                {
                    BookID = c.BookID,
                    BookTitle = c.BookTitle,
                    BSID = c.BSCategory.BSID,
                    BSName = c.BSCategory.BSName,
                    BLName = c.BSCategory.BLCategory.BLName,
                    BookPublish = c.BookPublish,
                    BookAuthor = c.BookAuthor,
                    BookContent = c.BookContent,
                    BookMoney = c.BookMoney,
                    BookPrice = c.BookPrice,
                    BookCount = c.BookCount,
                    BookDeport = c.BookDeport,
                    BookSale = c.BookSale
                }).ToList();
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        /// <summary>
        /// 基本书籍查询
        /// </summary>
        /// <param name="bk"></param>
        /// <returns></returns>
        public ActionResult SelBook(BooksModel bk)
        {
            using (DBEntities db = new DBEntities())
            {
                var q = db.Books.Where(c => c.BookIsDelete == false && c.BookIsHot == false && c.BookIsBuy == false);
                if (!string.IsNullOrEmpty(bk.BookTitle))
                {
                    q = q.Where(b => b.BookTitle.Contains(bk.BookTitle));
                }
                if (!string.IsNullOrEmpty(bk.BookPublish))
                {
                    q = q.Where(b => b.BookPublish.Contains(bk.BookPublish));
                }
                if (bk.BSID > 0)
                {
                    q = q.Where(b => b.BSID == bk.BSID);
                }
                var list = q.Select(c => new BooksModel
                {
                    BookID = c.BookID,
                    BookTitle = c.BookTitle,
                    BSID = c.BSCategory.BSID,
                    BSName = c.BSCategory.BSName,
                    BLName = c.BSCategory.BLCategory.BLName,
                    BookPublish = c.BookPublish,
                    BookAuthor = c.BookAuthor,
                    BookContent = c.BookContent,
                    BookMoney = c.BookMoney,
                    BookPrice = c.BookPrice,
                    BookCount = c.BookCount,
                    BookDeport = c.BookDeport,
                    BookSale = c.BookSale
                }).ToList();
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }

        /// <summary>
        /// 新增书籍
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public ActionResult AddBook(Books book)
        {
            using (DBEntities db = new DBEntities())
            {
                book.BookIsDelete = false;
                book.BookIsBuy = false;
                book.BookIsHot = false;
                db.Books.Add(book);
                int count = db.SaveChanges();
                return Json(count);
            }
        }
        /// <summary>
        /// 编辑书籍信息
        /// </summary>
        /// <param name="book"></param>
        /// <returns></returns>
        public ActionResult EditBook(Books book)
        {
            using (DBEntities db = new DBEntities())
            {
                var bk = db.Books.Where(b => b.BookID == book.BookID).FirstOrDefault();
                bk.BookTitle = book.BookTitle;
                bk.BSID = book.BSID;
                bk.BookPublish = book.BookPublish;
                bk.BookPrice = book.BookPrice;
                bk.BookMoney = book.BookMoney;
                bk.BookSale = book.BookSale;
                bk.BookDeport = book.BookDeport;
                int count = db.SaveChanges();
                return Json(count);
            }
        }
        #endregion

        #region 秒杀书籍
        /// <summary>
        /// 秒杀书籍
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBookBuy()
        {
            using (DBEntities db = new DBEntities())
            {
                var list = db.Books.Where(c => c.BookIsDelete == false && c.BookIsHot == false && c.BookIsBuy == true).Select(c => new BooksModel
                {
                    BookID = c.BookID,
                    BookTitle = c.BookTitle,
                    BSID = c.BSCategory.BSID,
                    BSName = c.BSCategory.BSName,
                    BLName = c.BSCategory.BLCategory.BLName,
                    BookPublish = c.BookPublish,
                    BookAuthor = c.BookAuthor,
                    BookContent = c.BookContent,
                    BookMoney = c.BookMoney,
                    BookPrice = c.BookPrice,
                    BookCount = c.BookCount,
                    BookDeport = c.BookDeport,
                    BookSale = c.BookSale,
                    BookBuyDate = c.BookBuyDate
                }).ToList();
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        /// <summary>
        /// 取消秒杀书籍
        /// </summary>
        /// <param name="bookid"></param>
        /// <returns></returns>
        public ActionResult DelBuy(int bookid)
        {
            using (DBEntities db = new DBEntities())
            {
                int count = 0;
                var book = db.Books.Where(b => b.BookID == bookid).FirstOrDefault();
                book.BookIsBuy = false;
                book.BookBuyDate = null;
                count = db.SaveChanges();
                return Json(count);
            }
        }
        /// <summary>
        /// 秒杀书籍查询
        /// </summary>
        /// <param name="bk"></param>
        /// <returns></returns>
        public ActionResult SelBookBuy(BooksModel bk)
        {
            using (DBEntities db = new DBEntities())
            {
                var q = db.Books.Where(c => c.BookIsDelete == false && c.BookIsHot == false && c.BookIsBuy == true);
                if (!string.IsNullOrEmpty(bk.BookTitle))
                {
                    q = q.Where(b => b.BookTitle.Contains(bk.BookTitle));
                }
                if (!string.IsNullOrEmpty(bk.BookPublish))
                {
                    q = q.Where(b => b.BookPublish.Contains(bk.BookPublish));
                }
                if (bk.BSID > 0)
                {
                    q = q.Where(b => b.BSID == bk.BSID);
                }
                var list = q.Select(c => new BooksModel
                {
                    BookID = c.BookID,
                    BookTitle = c.BookTitle,
                    BSID = c.BSCategory.BSID,
                    BSName = c.BSCategory.BSName,
                    BLName = c.BSCategory.BLCategory.BLName,
                    BookPublish = c.BookPublish,
                    BookAuthor = c.BookAuthor,
                    BookContent = c.BookContent,
                    BookMoney = c.BookMoney,
                    BookPrice = c.BookPrice,
                    BookCount = c.BookCount,
                    BookDeport = c.BookDeport,
                    BookSale = c.BookSale,
                    BookBuyDate= c.BookBuyDate,
                }).ToList();
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        /// <summary>
        /// 设置书籍为秒杀书籍
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public ActionResult AddBuy(int[] arr)
        {
            using (DBEntities db = new DBEntities())
            {
                int count = 0;
                //遍历bookid 设为秒杀书籍
                for (int i = 0; i < arr.Length; i++)
                {
                    var id = arr[i];
                    var book = db.Books.Where(b => b.BookID == id).FirstOrDefault();
                    book.BookIsBuy = true;
                    book.BookBuyDate = DateTime.Now;
                    count = db.SaveChanges();
                }
                return Json(count);
            }
        }
        #endregion

        #region 热门书籍
        /// <summary>
        /// 热门书籍
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBookHot()
        {
            using (DBEntities db = new DBEntities())
            {
                var list = db.Books.Where(c => c.BookIsDelete == false && c.BookIsHot == true && c.BookIsBuy == false).Select(c => new BooksModel
                {
                    BookID = c.BookID,
                    BookTitle = c.BookTitle,
                    BSID = c.BSCategory.BSID,
                    BSName = c.BSCategory.BSName,
                    BLName = c.BSCategory.BLCategory.BLName,
                    BookPublish = c.BookPublish,
                    BookAuthor = c.BookAuthor,
                    BookContent = c.BookContent,
                    BookMoney = c.BookMoney,
                    BookPrice = c.BookPrice,
                    BookCount = c.BookCount,
                    BookDeport = c.BookDeport,
                    BookSale = c.BookSale,
                    BookHotDate = c.BookHotDate
                }).ToList();
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        /// <summary>
        /// 热门书籍查询
        /// </summary>
        /// <param name="bk"></param>
        /// <returns></returns>
        public ActionResult SelBookHot(BooksModel bk)
        {
            using (DBEntities db = new DBEntities())
            {
                var q = db.Books.Where(c => c.BookIsDelete == false && c.BookIsHot == true && c.BookIsBuy == false);
                if (!string.IsNullOrEmpty(bk.BookTitle))
                {
                    q = q.Where(b => b.BookTitle.Contains(bk.BookTitle));
                }
                if (!string.IsNullOrEmpty(bk.BookPublish))
                {
                    q = q.Where(b => b.BookPublish.Contains(bk.BookPublish));
                }
                if (bk.BSID > 0)
                {
                    q = q.Where(b => b.BSID == bk.BSID);
                }
                var list = q.Select(c => new BooksModel
                {
                    BookID = c.BookID,
                    BookTitle = c.BookTitle,
                    BSID = c.BSCategory.BSID,
                    BSName = c.BSCategory.BSName,
                    BLName = c.BSCategory.BLCategory.BLName,
                    BookPublish = c.BookPublish,
                    BookAuthor = c.BookAuthor,
                    BookContent = c.BookContent,
                    BookMoney = c.BookMoney,
                    BookPrice = c.BookPrice,
                    BookCount = c.BookCount,
                    BookDeport = c.BookDeport,
                    BookSale = c.BookSale,
                    BookHotDate = c.BookHotDate
                }).ToList();
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        /// <summary>
        /// 设置书籍为热门书籍
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        public ActionResult AddHot(int[] arr)
        {
            using (DBEntities db = new DBEntities())
            {
                int count = 0;
                //遍历bookid 设为热门书籍
                for (int i = 0; i < arr.Length; i++)
                {
                    var id = arr[i];
                    var book = db.Books.Where(b => b.BookID == id).FirstOrDefault();
                    book.BookIsHot = true;
                    book.BookHotDate = DateTime.Now;
                    count = db.SaveChanges();
                }
                return Json(count);
            }
        }
        /// <summary>
        /// 取消热门书籍
        /// </summary>
        /// <param name="bookid"></param>
        /// <returns></returns>
        public ActionResult DelHot(int bookid)
        {
            using (DBEntities db = new DBEntities())
            {
                int count = 0;
                var book = db.Books.Where(b => b.BookID == bookid).FirstOrDefault();
                book.BookIsHot = false;
                book.BookHotDate = null;
                count = db.SaveChanges();
                return Json(count);
            }
        }
        #endregion

        #region 书籍回收站

        /// <summary>
        /// 书籍回收站
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBookDel()
        {
            using (DBEntities db = new DBEntities())
            {
                var list = db.Books.Where(c => c.BookIsDelete == true).Select(c => new BooksModel
                {
                    BookID = c.BookID,
                    BookTitle = c.BookTitle,
                    BSID = c.BSCategory.BSID,
                    BSName = c.BSCategory.BSName,
                    BLName = c.BSCategory.BLCategory.BLName,
                    BookPublish = c.BookPublish,
                    BookAuthor = c.BookAuthor,
                    BookContent = c.BookContent,
                    BookMoney = c.BookMoney,
                    BookPrice = c.BookPrice,
                    BookCount = c.BookCount,
                    BookDeport = c.BookDeport,
                    BookSale = c.BookSale
                }).ToList();
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }
        /// <summary>
        /// 添加到回收站
        /// </summary>
        /// <param name="bookid"></param>
        /// <returns></returns>
        public ActionResult AddDel(int bookid)
        {
            using (DBEntities db = new DBEntities())
            {
                var book = db.Books.Where(b => b.BookID == bookid).FirstOrDefault();
                book.BookIsDelete = true;
                int count = db.SaveChanges();
                return Json(count);
            }
        }

        /// <summary>
        /// 回收站书籍查询
        /// </summary>
        /// <param name="bk"></param>
        /// <returns></returns>
        public ActionResult SelBookDel(BooksModel bk)
        {
            using (DBEntities db = new DBEntities())
            {
                var q = db.Books.Where(c => c.BookIsDelete == false && c.BookIsHot == false && c.BookIsBuy == false);
                if (!string.IsNullOrEmpty(bk.BookTitle))
                {
                    q = q.Where(b => b.BookTitle.Contains(bk.BookTitle));
                }
                if (!string.IsNullOrEmpty(bk.BookPublish))
                {
                    q = q.Where(b => b.BookPublish.Contains(bk.BookPublish));
                }
                if (bk.BSID > 0)
                {
                    q = q.Where(b => b.BSID == bk.BSID);
                }
                var list = q.Select(c => new BooksModel
                {
                    BookID = c.BookID,
                    BookTitle = c.BookTitle,
                    BSID = c.BSCategory.BSID,
                    BSName = c.BSCategory.BSName,
                    BLName = c.BSCategory.BLCategory.BLName,
                    BookPublish = c.BookPublish,
                    BookAuthor = c.BookAuthor,
                    BookContent = c.BookContent,
                    BookMoney = c.BookMoney,
                    BookPrice = c.BookPrice,
                    BookCount = c.BookCount,
                    BookDeport = c.BookDeport,
                    BookSale = c.BookSale
                }).ToList();
                var jsonResult = Json(list, JsonRequestBehavior.AllowGet);
                jsonResult.MaxJsonLength = int.MaxValue;
                return jsonResult;
            }
        }


        /// <summary>
        /// 移除书籍回收站
        /// </summary>
        /// <param name="bookid"></param>
        /// <returns></returns>
        public ActionResult DelHSz(int bookid)
        {
            using (DBEntities db = new DBEntities())
            {
                int count = 0;
                var book = db.Books.Where(b => b.BookID == bookid).FirstOrDefault();
                book.BookIsDelete = false;
                count = db.SaveChanges();
                return Json(count);
            }
        }
        /// <summary>
        /// 彻底删除
        /// </summary>
        /// <param name="bookid"></param>
        /// <returns></returns>
        public ActionResult CDDel(int bookid) 
        {
            using (DBEntities db = new DBEntities())
            {
                var book = db.Books.Where(b => b.BookID == bookid).FirstOrDefault();
                db.Books.Remove(book);
                int count = db.SaveChanges();
                return Json(count);
            }
        }
        #endregion

    }
}