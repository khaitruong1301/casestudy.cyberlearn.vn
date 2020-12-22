using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Service.ViewModels.Task
{
    public class TaskViewModel
    {
    }

    public class UpdateStatusVM
    {
        public int taskId { get; set; }
        public string statusId { get; set; }
    }

    public class UpdatePiority
    {
        public int taskId { get; set; }
        public int priorityId { get; set; }
    }


    public class UpdateStatus
    {
        public int taskId { get; set; }
        public string statusId { get; set; }

    }


    public class UpdateDescription
    {
        public int taskId { get; set; }
        public string description { get; set; }
    }

    public class TimeTrackingUpdate
    {
        public int taskId { get; set; }

        public int timeTrackingSpent { get; set; }
        public int timeTrackingRemaining { get; set; }
    }
    public class updateEstimate
    {
        public int taskId { get; set; }

        public int originalEstimate { get; set; }
    }


    public class taskInsert
    {

        public string taskName { get; set; }
        public string description { get; set; }
        public string statusId { get; set; }

        public int originalEstimate { get; set; }

        public int timeTrackingSpent { get; set; }
        public int timeTrackingRemaining { get; set; }

        public int projectId { get; set; }
        public int typeId { get; set; }
        public int priorityId { get; set; }
        public List<int> listUserAsign = new List<int>();

    }
    public class TaskEdit
    {
        public string taskId { get; set; }
        public string taskName { get; set; }

        public string description { get; set; }
        public string statusId { get; set; }
        public int originalEstimate { get; set; }
        public int timeTrackingSpent { get; set; }
        public int timeTrackingRemaining { get; set; }
        public int projectId { get; set; }
        public int typeId { get; set; }
        public int priorityId { get; set; }
        public List<int> listUserAsign = new List<int>();
    }

  
}