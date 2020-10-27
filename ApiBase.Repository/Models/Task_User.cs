using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Models
{
    public class Task_User
    {
        public int id { get; set; }
        public int useId { get; set; }
        public int taskId { get; set; }
        public bool deleted { get; set; }


    }
}
