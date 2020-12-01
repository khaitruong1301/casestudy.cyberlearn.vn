using ApiBase.Repository.Infrastructure;
using ApiBase.Repository.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Repository
{
    public interface IProject_UserReponsitory : IRepository<Project_User>
    {

    }
    public class Project_UserReponsitory : RepositoryBase<Project_User>, IProject_UserReponsitory
    {
        public Project_UserReponsitory(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
}
