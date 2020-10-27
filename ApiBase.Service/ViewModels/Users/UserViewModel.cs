using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Service.ViewModels.Users
{
    public class UserViewModel
    {
        public string email { get; set; }

        public string name { get; set; }
        public string password { get; set; }

        public bool gender { get; set; }
        public string phone { get; set; }
        public string facebookId { get; set; }
        public string userTypeId { get; set; }
        public bool deleted { get; set; }
        public string avatar { get; set; }
    }

    public class InfoUser
    {
        public string email { get; set; }
        public string password { get; set; }

        public string name { get; set; }
        public bool gender { get; set; }
        public string phone { get; set; }
    }
    public class UserLogin
    {
        public string email { get; set; }
        public string password { get; set; }

    }
    public class UserLoginResult { 
        public string email { get; set; }
        public string accessToken { get; set; }

    }
}
