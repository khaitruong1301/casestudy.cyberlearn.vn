using ApiBase.Repository.Infrastructure;
using ApiBase.Repository.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApiBase.Repository.Repository
{
    public interface IProjectCategoryRepository : IRepository<ProjectCategory>
    {

    }

    public class ProjectCategoryRepository : RepositoryBase<ProjectCategory>, IProjectCategoryRepository
    {
        public ProjectCategoryRepository(IConfiguration configuration)
            : base(configuration)
        {

        }
    }
   
}
