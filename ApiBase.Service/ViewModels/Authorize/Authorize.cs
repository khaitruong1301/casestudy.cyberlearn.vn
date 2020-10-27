using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Service.ViewModels.Authorize
{
    public class UserTypeViewModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool deleted { get; set; }
    }

    public class UserType_RoleViewModel
    {
        public string userTypeId { get; set; }
        public string roleId { get; set; }

        public string description { get; set; }
        public string deleted { get; set; }


    }

    public class RoleViewModel
    {
        public string id { get; set; }
        public string name { get; set; }

        public string description { get; set; }


    }
}
