using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiBase.Repository.Models;
using ApiBase.Repository.Repository;
using ApiBase.Service.Services;
using ApiBase.Service.Services.CommentService;
using ApiBase.Service.Services.TaskTypeService;
using ApiBase.Service.Services.UserService;
using ApiBase.Service.ViewModels;
using ApiBase.Service.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiBase.Api.Controllers
{
    [Route("api/[controller]")]
    public class TaskTypeController : ControllerBase
    {
        ITaskTypeService _taskTypeService;
        public TaskTypeController(ITaskTypeService taskTypeService)
        {
            _taskTypeService = taskTypeService;
        }


        [HttpGet("getAll")]
                            
        public async Task<IActionResult> getAll()
        {

            var result = await _taskTypeService.getAllTaskType();
            return result;
        }
    }
}

