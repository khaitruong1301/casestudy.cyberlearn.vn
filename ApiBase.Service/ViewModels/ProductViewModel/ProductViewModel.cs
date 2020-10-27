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
        public bool feature { get; set; }
        public string image { get; set; }


    }


    public class ProductDetailViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string alias { get; set; }
        public decimal price { get; set; }
        public bool feature { get; set; }
        public string description { get; set; }
        public List<string> size { get; set; }
        public string shortDescription { get; set; }
        public int quantity { get; set; }
        public string image { get; set; }
        public List<ProCategory> categories { get; set; }
        public List<ProductRelationViewModel>  relatedProducts { get; set; }
    }
    public class ProductRelationViewModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string alias { get; set; }
        public bool feature { get; set; }
        public decimal price { get; set; }
        public string description { get; set; }
        public string shortDescription { get; set; }
        public string image { get; set; }
   
    }
    public class ProductCategory
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool feature { get; set; }
        public string alias { get; set; }
         
    }

    public class ProCategory {
        public string id { get; set; }
        public string category { get; set; }
    }
}
