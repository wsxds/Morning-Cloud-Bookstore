using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Morning_Cloud_Bookstore.Models.Model
{
    public class BLCategoryModel
    {
        public int BLID { get; set; }
        public string BLName { get; set; }
        public List<BSCategoryModel> bslist { get; set; } = new List<BSCategoryModel>();

    }
}