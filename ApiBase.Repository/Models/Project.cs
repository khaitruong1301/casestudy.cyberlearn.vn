using System.Collections.Generic;
using System.Reflection;

namespace ApiBase.Repository.Models
{
    public class Project
    {
        public int id { get; set; }
        public string projectName { get; set; }
        public string description { get; set; }
        public int categoryId { get; set; }
        public string alias { get; set; }
        public bool deleted { get; set; }
        public int creator { get; set; }
    }

    public class ProjectInsert
    {
        public string projectName { get; set; }
        public string description { get; set; }
        public int categoryId { get; set; }
        public string alias { get; set; }
    }



    public class ProjectDetail
    {
        public int id { get; set; }
        public string projectName { get; set; }
        public string description { get; set; }
        public ProjectCategoryDetail projectCategory { get; set; }
        public string alias { get; set; }
        public List<StatusTask> lstTask = new List<StatusTask>();
        public CreatorModel creator = new CreatorModel();

    }

    public class CreatorModel
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class ProjectCategoryDetail {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class TaskDetail
    {
        public int taskId { get; set; }
        public string taskName { get; set; }
        public string alias { get; set; }
        public string description { get; set; }
        public string statusId { get; set; }
        public int originalEstimate { get; set; }
        public int timeTracking { get; set; }

        public TaskPriority priorityTask = new TaskPriority();

        public TaskTypeDetail taskTypeDetail = new TaskTypeDetail();


        public List<userAssign> assigness = new List<userAssign>();

        public List<CommentTask> lstComment = new List<CommentTask>();

    }

    public class TaskTypeDetail { 
        public int id { get; set; }
        public string taskType { get; set; }
    }
    public class userAssign
    {
        public int id { get; set; }
        public string avatar { get; set; }
        public string name { get; set; }
        public string alias { get; set; }
    }
    public class Reporter
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class TaskPriority
    {
        public int priorityId { get; set; }
        public string priority { get; set; }

    }
    public class StatusTask
    {

        public string statusId { get; set; }
        public string statusName { get; set; }
        public string alias { get; set; }
        public List<TaskDetail> lstTaskDeTail = new List<TaskDetail>();
    }


    public class CommentTask
    {
        public int idUser { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
        public string commentContent { get; set; }

    }
}
