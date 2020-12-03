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
using ApiBase.Service.ViewModels.ProjectViewModel;
using ApiBase.Service.ViewModels.Task;

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


        [Authorize]
        [HttpPut("updateProject")]
        public async Task<IActionResult> updateProject(int? projectId, ProjectUpdate projectUpdate)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];

            return await _projectService.updateProject(projectId,projectUpdate, accessToken);
        }

        [Authorize]
        [HttpPost("assignUserProject")]
        public async Task<IActionResult> assignUserProject([FromBody] UserProject project)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];

            return await _projectService.addUserProject(project, accessToken);

        }
        [Authorize]
        [HttpPost("assignUserTask")]
        public async Task<IActionResult> assignUserTask([FromBody] TaskUser taskUser)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];

            return await _projectService.addTaskUser(taskUser, accessToken);

        }

        [Authorize]
        [HttpPost("removeUserFromTask")]
        public async Task<IActionResult> removeUserFromTask([FromBody] TaskUser taskUser)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];

            return await _projectService.removeUserFromTask(taskUser, accessToken);

        }


        [Authorize]
        [HttpPost("removeUserFromProject")]
        public async Task<IActionResult> removeUserFromProject([FromBody] Project_User project)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];

            return await _projectService.removeUSerFromProject(project, accessToken);

        }


        [Authorize]
        [HttpPut("updateStatus")]
        public async Task<IActionResult> updateStatus([FromBody] UpdateStatusVM model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            return await _projectService.updateStatusTask(model, accessToken);
        }


        [Authorize]
        [HttpPut("updatePiority")]
        public async Task<IActionResult> updatePiority([FromBody] UpdatePiority model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            return await _projectService.updatePiority(model, accessToken);
        }

        [Authorize]
        [HttpPut("updateDescription")]
        public async Task<IActionResult> updateDescription(UpdateDescription model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            return await _projectService.updateDescription(model, accessToken);
        }


        [Authorize]
        [HttpPut("updateTimeTracking")]
        public async Task<IActionResult> updateTimeTracking(TimeTrackingUpdate model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            return await _projectService.updateTimeTracking(model, accessToken);
        }

        [Authorize]
        [HttpPut("updateEstimate")]
        public async Task<IActionResult> updateEstimate(updateEstimate model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            return await _projectService.updateEstimate(model, accessToken);
        }


        [Authorize]
        [HttpPost("createTask")] 
        public async Task<IActionResult> createTask (taskInsert model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            return await _projectService.createTask(model, accessToken);
        }

        [Authorize]
        [HttpDelete("removeTask")]
        public async Task<IActionResult> removeTask(int taskId)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            return await _projectService.removeTask(taskId, accessToken);
        }
    }
}
