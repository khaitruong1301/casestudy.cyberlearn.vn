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
using Microsoft.Net.Http.Headers;

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
            return await _projectService.createProject(model,null);
        }

        [Authorize]
        [HttpPost("createProjectAuthorize")]
        public async Task<IActionResult> createProjectAuthorize([FromBody] ProjectInsert model)
        {   
            var accessToken = Request.Headers[HeaderNames.Authorization];

            return await _projectService.createProject(model, accessToken);
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
            return await _projectService.getAllProject();
        }

        [Authorize]
        [HttpDelete("deleteProject")]
        public async Task<IActionResult> deleteProject(int projectId)
        {
            List<dynamic> lstId = new List<dynamic>();
            lstId.Add(projectId);
            return await _projectService.DeleteByIdAsync(lstId);
        }

    }
}
