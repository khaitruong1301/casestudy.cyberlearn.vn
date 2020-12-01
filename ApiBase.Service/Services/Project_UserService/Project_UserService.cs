using ApiBase.Repository.Models;
using ApiBase.Repository.Repository;
using ApiBase.Service.Infrastructure;
using ApiBase.Service.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApiBase.Service.Services.Project_UserService
{
    public interface IProject_UserService : IService<Project_User, Project_User>
    {

    }
    public class Project_UserService : ServiceBase<Project_User, Project_User>, IProject_UserService
    {
        public Project_UserService(Project_UserReponsitory proRe,
          IMapper mapper)
          : base(proRe, mapper)
        {
          
        }
    }
}