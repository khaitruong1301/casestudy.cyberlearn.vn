using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Service.ViewModels.ProjectViewModel
{


    public class ProjectViewModelResult
    {
        public int id { get; set; }
        public string projectName { get; set; }
        public string description { get; set; }
        public int categoryId { get; set; }
        public string categoryName { get; set; }
        public string alias { get; set; }
        public bool deleted { get; set; }
        public List<Member> members = new List<Member>();

        public Creator creator = new Creator();
    }

    public class Member
    {
        public int userId { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
    }

    public class Creator
    {
        public int id { get; set; }
        public string name { get; set; }
    }

   
}
