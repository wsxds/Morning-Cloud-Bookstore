using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Morning_Cloud_Bookstore.Models.Model
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserPwd { get; set; }
        public string UserEmail { get; set; }
        public string UserSex { get; set; }
        public string MyProperty { get; set; }
        public string UserNick { get; set; }
    }
}