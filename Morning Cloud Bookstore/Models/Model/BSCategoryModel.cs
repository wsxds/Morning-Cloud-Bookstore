using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Morning_Cloud_Bookstore.Models.Model
{
    public class BSCategoryModel
    {
        public int BSID { get; set; }
        public Nullable<int> BLID { get; set; }
        public string BSName { get; set; }
        public string BLName { get; set; }

    }
}