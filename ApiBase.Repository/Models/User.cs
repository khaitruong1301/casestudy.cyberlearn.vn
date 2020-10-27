using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Models
{
    public class AppUser
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
}
