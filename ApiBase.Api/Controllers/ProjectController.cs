using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiBase.Repository.Models;
using ApiBase.Service.Services.PriorityService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace ApiBase.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        IProjectService _projectService;
        public ProjectController(IProjectService projectService )
        {
            _projectService = projectService;
        }


        [HttpPost("createProject")]
        public async Task<IActionResult> createProject([FromBody] ProjectInsert model)
        {
            return await _projectService.createProject(model);
        }

        [Authorize]
        [HttpPost("createProjectAuthorize")]
        public async Task<IActionResult> createProjectAuthorize([FromBody] ProjectInsert model)
        {
            return await _projectService.createProject(model);
        }
        [Authorize]

        [HttpGet("getProjectDetail")]
        public async Task<IActionResult> getProjectDetail(int id)
        {
            return await _projectService.getProjectById(id);
        }
        [Authorize]

        [HttpGet("getAllProject")]
        public async Task<IActionResult> getAllProject()
        {
            return await _projectService.GetAllAsync();
        }

    }
}
