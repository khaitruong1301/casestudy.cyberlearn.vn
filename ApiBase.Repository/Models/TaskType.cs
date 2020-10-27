using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Models
{
    public class TaskType
    {
        public int id { get; set; }
        public string taskType { get; set; }
        public string deleted { get; set; }
        public string alias { get; set; }

    }
}
