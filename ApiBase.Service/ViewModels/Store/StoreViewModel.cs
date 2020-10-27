using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Service.ViewModels.Store
{
    public class StoreViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string alias { get; set; }
        public string latitude { get; set; }
        public string longtitude { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public bool deleted { get; set; }
    }
}
