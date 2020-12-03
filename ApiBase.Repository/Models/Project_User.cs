using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ApiBase.Repository.Models
{
    public class Project_User
    {
        public  int id {get;set ;}
        [DisplayName("alias")]
        public string alias { get; set; }

        public int projectId { get; set; }
        public int userId { get; set; } 
        public bool deleted { get; set; }
    }

    public class UserProject
    {
    

        public int projectId { get; set; }
        public int userId { get; set; }
    }




    public class TaskUser
    {
        public int taskId { get; set; }
        public int userId { get; set; }
    }
}
