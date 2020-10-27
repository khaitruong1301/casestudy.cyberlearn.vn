using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Models
{
    public class Status
    {
        public string statusId { get; set; }
        public string statusName { get; set; }
        public string alias { get; set; }
        public string deleted { get; set; }
    }
}
