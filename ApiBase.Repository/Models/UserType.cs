using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Models
{
    public class UserType
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool deleted { get; set; }
    }
}
