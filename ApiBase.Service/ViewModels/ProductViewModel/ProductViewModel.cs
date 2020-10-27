using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Service.ViewModels.ProductViewModel
{
    public class ProductViewModel
    {

        public string id { get; set; }
        public string name { get; set; }
        public string alias { get; set; }
        public decimal price { get; set; }
        public string description { get; set; }
        public string size { get; set; }
        public string shortDescription { get; set; }
        public int quantity { get; set; }
        public bool deleted { get; set; }
        public string categories { get; set; }
        public string relatedProducts { get; set; }

    }
}
