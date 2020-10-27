using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiBase.Repository.Repository;
using ApiBase.Service.Services;
using ApiBase.Service.Services.UserService;
using ApiBase.Service.ViewModels;
using ApiBase.Service.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiBase.Api.Controllers
{
    [Route("api/[controller]")]
    public class ProjectCategoryController : ControllerBase
    {
        IProjectCategoryService _projectCategoryService;


        public ProjectCategoryController(IProjectCategoryService projectCategoryService)
        {
            _projectCategoryService = projectCategoryService;
        }


        [HttpGet]
        public async Task<IActionResult> getAll()
        {

            var result = await _projectCategoryService.GetAllAsync();
            return result;
        }
        

    }
}
