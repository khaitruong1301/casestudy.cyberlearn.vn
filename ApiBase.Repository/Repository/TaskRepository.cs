using ApiBase.Repository.Infrastructure;
using ApiBase.Repository.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Repository
{
    public interface ITaskRepository: IRepository<Task>
    {

    }   
    public class TaskRepository : RepositoryBase<Task>, ITaskRepository
    {
        public TaskRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
