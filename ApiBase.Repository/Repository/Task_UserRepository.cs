using ApiBase.Repository.Infrastructure;
using ApiBase.Repository.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Repository
{
 
    public interface ITask_UserRepository : IRepository<Task_User>
    {

    }
    public class Task_UserRepository : RepositoryBase<Task_User>, ITask_UserRepository
    {
        public Task_UserRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
