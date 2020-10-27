using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Models
{
    public class Category
    {
        public string id { get; set; }
        public string category { get; set; }
        public string categoryParent { get; set; }
        public string categoryChild { get; set; }
        public bool deleted { get; set; }
        public string productList { get; set; }
        public string alias { get; set; }
    }
}
