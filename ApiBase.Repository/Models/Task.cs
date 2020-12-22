using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Models
{
    public class Task
    {
        public int taskId { get; set; }
        public string taskName { get; set; }
        public string alias { get; set; }
        public string description { get; set; }
        public string statusId { get; set; }
        public int originalEstimate { get; set; }
        public int timeTrackingSpent { get; set; }
        public int timeTrackingRemaining { get; set; }
        public int projectId { get; set; }
        public int typeId { get; set; }
        public bool deleted { get; set; }
        public int reporterId { get; set; }
        public int priorityId { get; set; }
    }

}
