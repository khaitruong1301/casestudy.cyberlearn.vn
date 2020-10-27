using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Models
{
    public class OrderProduct
    {
        public int id { get; set; }
        public string orderDetail { get; set; }
        public DateTime date { get; set; }

        public string status { get; set; }
        public string email { get; set; }

        public bool deleted { get; set; }
        public string alias { get; set; }
    }
}
