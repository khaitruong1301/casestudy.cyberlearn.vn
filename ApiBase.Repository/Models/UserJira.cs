using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Models
{
    public class UserJira
    {
        public int id { get; set; }
        public string name { get; set; }
        public string passWord { get; set; }
        public string avatar { get; set; }
        public string email { get; set; }
        public string phoneNumber { get; set; }
        public bool deleted { get; set; }
        public string alias { get; set; }
        public string facebookId { get; set; }
    }
    public class UserJiraModel
    {
        public string email { get; set; }
        public string passWord { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set;}
    }
    public class UserJiraModelUpdate
    {
        public string id { get; set; }

        public string passWord { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public string phoneNumber { get; set; }
    }


    public class UserJiraLogin
    {
        public string email { get; set; }
        public string passWord { get; set; }
   
    }

    public class UserJiraModelView
    {
        public int id { get; set; }
        public string email { get; set; }
        public string avatar { get; set; }
        public string phoneNumber { get; set; }

        public string name { get; set;
        }
        public string accessToken { get; set; }
    }
}
