using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Morning_Cloud_Bookstore.Models.Model
{
    public class OrdersModel
    {
        public int OrderID { get; set; }
        public int? UserID { get; set; }
        public int AMID { get; set; }
        public string OrderNum { get; set; }
        public DateTime? OrderDate { get; set; }
        public int? OrderState { get; set; }
        public decimal? OrderMoney { get; set; }
        public string UserNick { get; set; }
        public string UserName { get; set; }
        public string AMTel { get; set; }
        public string AMAddress { get; set; }
    }
}