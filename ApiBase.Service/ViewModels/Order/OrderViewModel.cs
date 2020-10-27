using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Service.ViewModels.Order
{
    public class OrderViewModel
    {
        public List<OrderDetail> orderDetail = new List<OrderDetail>();
        public string email { get; set; }
       
          
    }


    public class OrderDetail
    {
        public string productId { get; set; }
        public decimal quantity { get; set; }

    }
    public class OrderDestroy
    {
        public int orderId { get; set; }
    }
}
