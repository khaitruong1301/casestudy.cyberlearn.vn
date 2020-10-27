using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Models
{
    public class Priority
    {
        public int priorityId { get; set; }
        public string priority { get; set; }
        public string description { get; set; }
        public bool deleted { get; set; }
        public string alias { get; set; }

    }
}
