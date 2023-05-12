using Morning_Cloud_Bookstore.Models;
using Morning_Cloud_Bookstore.Models.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Morning_Cloud_Bookstore.Controllers
{
    public class CategoryController : Controller
    {
        // GET: Category
        public ActionResult Index()
        {
            return View();
        }

        #region 大类别
        public ActionResult BLIndex()
        {
            return View();
        }
        public static List<BLCategoryModel> getBLAll(DBEntities db)
        {
            var list = db.BLCategory.Select(c => new BLCategoryModel
            {
                BLID = c.BLID,
                BLName = c.BLName
            }).ToList();
            return list;
        }

        //获取所有大类别
        public ActionResult GetBLCategoryAll()
        {
            using (DBEntities db = new DBEntities())
            {
                var list = getBLAll(db);
                list.Insert(0, new BLCategoryModel { BLID = 0, BLName = "全部" });
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="BLName"></param>
        /// <returns></returns>
        public ActionResult AddBLCategory(string BLName)
        {
            using (DBEntities db = new DBEntities())
            {
                BLCategory bl = new BLCategory();
                bl.BLName = BLName;
                db.BLCategory.Add(bl);
                db.SaveChanges();
                var list = getBLAll(db);
                return Json(list);
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="bl"></param>
        /// <returns></returns>
        public ActionResult EditBLCategory(BLCategory bl)
        {
            using (DBEntities db = new DBEntities())
            {
                var r = db.BLCategory.Where(c => c.BLID == bl.BLID).FirstOrDefault();
                r.BLName = bl.BLName;
                db.SaveChanges();
                var list = getBLAll(db);
                return Json(list);
            }
        }
        /// <summary>
        /// 大类别删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DelBLCategory(int id)
        {
            using (DBEntities db = new DBEntities())
            {
                int num = 0;
                var r = db.BSCategory.Where(c => c.BLID == id).ToList();
                if (r.Count > 0)
                {
                    num = -1;
                }
                else
                {

                    var bl = db.BLCategory.Where(c => c.BLID == id).FirstOrDefault();
                    db.BLCategory.Remove(bl);
                    db.SaveChanges();
                }
                var list = getBLAll(db);
                return Json(new { list, num });
            }
        }
        #endregion


        #region 小类别
        private List<BSCategoryModel> GetBSCategory(DBEntities db)
        {
            var list = db.BSCategory.Select(c => new BSCategoryModel
            {
                BSID = c.BSID,
                BSName = c.BSName,
                BLID = c.BLID,
                BLName = c.BLCategory.BLName
            }).ToList();
            return list;
        }
        public ActionResult BSIndex()
        {
            return View();
        }
        //获取所有小类别
        public ActionResult GetBSCategoryAll()
        {
            using (DBEntities db = new DBEntities())
            {
                var list = GetBSCategory(db);
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// 根据大类别id查询小类别
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBSCategoryByBLID(int blid)
        {
            using (DBEntities db = new DBEntities())
            {
                var r = db.BSCategory.Where(b=>b.BSID >-1);
                if (blid > 0)
                {
                    r = r.Where(c => c.BLID == blid);
                }
                var list = r.Select(c => new BSCategoryModel
                {
                    BSID = c.BSID,
                    BSName = c.BSName,
                    BLID = c.BLID,
                    BLName = c.BLCategory.BLName
                }).ToList();
                return Json(list);
            }
        }
        /// <summary>
        /// 小类别编辑
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public ActionResult EditBSCategory(BSCategory bs)
        {
            using (DBEntities db = new DBEntities())
            {
                var r = db.BSCategory.Single(c => c.BSID == bs.BSID);
                r.BSName = bs.BSName;
                r.BLID = bs.BLID;
                db.SaveChanges();
                var list = GetBSCategory(db);
                return Json(list);
            }
        }
        /// <summary>
        /// 小类别删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult DelBSCategory(int id)
        {
            using (DBEntities db = new DBEntities())
            {
                int num = 0;
                var r = db.Books.Where(c => c.BSID == id).ToList();
                if (r.Count > 0)
                {
                    num = -1;
                }
                else
                {

                    var bs = db.BSCategory.Where(c => c.BSID == id).FirstOrDefault();
                    db.BSCategory.Remove(bs);
                    db.SaveChanges();
                }

                var list = GetBSCategory(db);
                return Json(new { list, num });
            }
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="bs"></param>
        /// <returns></returns>
        public ActionResult AddBSCategory(BSCategory bs)
        {
            using (DBEntities db = new DBEntities())
            {
                db.BSCategory.Add(bs);
                db.SaveChanges();
                var list = GetBSCategory(db);
                return Json(list);
            }
        }
        #endregion



    }
}