using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Morning_Cloud_Bookstore.Models.Model
{
    public class BooksModel
    {
        public int ID { get; set; }
        public string TopUrl { get; set; }
        public string ImgUrl { get; set; }
        public int BookID { get; set; }
        public int BSID { get; set; }
        public string BSName { get; set; }
        public string BLName { get; set; }
        public string BookTitle { get; set; }
        public string BookAuthor { get; set; }
        public string BookPublish { get; set; }
        public string ISBN { get; set; }
        public int? BookCount { get; set; }
        public decimal? BookMoney { get; set; }
        public decimal? BookPrice { get; set; }
        public string BookDesc { get; set; }
        public string BookAuthorDesc { get; set; }
        public string BookComm { get; set; }
        public string BookContent { get; set; }
        public int? BookSale { get; set; }
        public int? BookDeport { get; set; }
        public bool BookIsBuy { get; set; }
        public bool BookIsHot { get; set; }
        public bool BookIsDelete { get; set; }
        public DateTime? BookBuyDate { get; set; }
        public DateTime? BookHotDate { get; set; }
        
        public decimal? XiaoJi { get; set; }
        public int? Count { get; set; }
    }
}