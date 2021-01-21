using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Models
{
    public class Comment
    {
        public int id { get; set; }
        public int userId { get; set; }
        public int taskId { get; set; }
        public string contentComment { get; set; }
        public bool deleted { get; set; }
        public string alias { get; set; }
    }

    public class CommentViewModel
    {
        public int id { get; set; }
        public int userId { get; set; }
        public int taskId { get; set; }
        public string contentComment { get; set; }
        public bool deleted { get; set; }
        public string alias { get; set; }

        public UserComment user = new UserComment();
    }

    public class UserComment
    {
        public int userId { get; set; }
        public string name { get; set; }
        public string avatar { get; set; }
    }

    public class CommentModel
    {
        public int id { get; set; }
        public int userId { get; set; }
        public string contentComment { get; set; }
    }

    public class CommentModelInsert
    {
        public int taskId { get; set; }
        public string contentComment { get; set; }
    }

    public class CommentModelUpdate
    {
        public int id { get; set; }

        public string contentComment { get; set; }

    }
    public class CommentModelDelete
    {
        public int id { get; set; }

    }
}
