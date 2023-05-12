using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Morning_Cloud_Bookstore.Models.Model
{
    public class OrderDetailModel
    {
        public string BookTitle { get; set; }
        public int ODID { get; set; }
        public int OrderID { get; set; }
        public int BookID { get; set; }
        public decimal? ODPrice { get; set; }
        public int? ODCount { get; set; }
        public decimal? ODMoney { get; set; }
    }
}