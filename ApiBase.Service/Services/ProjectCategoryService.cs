using ApiBase.Repository.Models;
using ApiBase.Repository.Repository;
using ApiBase.Service.Infrastructure;
using ApiBase.Service.ViewModels;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApiBase.Service.Services
{
    public interface IProjectCategoryService : IService<ProjectCategory, ProjectCategory>
    {
        Task<ResponseEntity> getAll();
    }

    public class ProjectCategoryService: ServiceBase<ProjectCategory, ProjectCategory>, IProjectCategoryService
    {
        IProjectCategoryRepository _projectCategoryRepository;

        public ProjectCategoryService(IProjectCategoryRepository projectCategory,
            IMapper mapper)
            : base(projectCategory, mapper)
        {
            _projectCategoryRepository = projectCategory;
        }

        public Task<ResponseEntity> getAll()
        {
            throw new NotImplementedException();
        }
    }

}
