using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiBase.Repository.Models;
using ApiBase.Repository.Repository;
using ApiBase.Service.Services;
using ApiBase.Service.Services.CommentService;
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
    public class CommentController : ControllerBase
    {
        ICommentService _commentService;


        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }


        [HttpGet("getAll")]
       
        public async Task<IActionResult> getAll(int taskId)
        {

            var result = await _commentService.getCommentByTask(taskId);
            return result;
        }

        [HttpPost("insertComment")]

        [Authorize]
        public async Task<IActionResult> insertComment([FromBody] CommentModelInsert model)
        {

            var accessToken = Request.Headers[HeaderNames.Authorization];

            var result = await _commentService.insertComment(model, accessToken);
            return result;
        }

        [HttpPut("updateComment/{idComment}")]

        [Authorize]

        public async Task<IActionResult> updateComment ( int idComment,CommentModelUpdate model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            var result = await _commentService.updateComment(model, idComment, accessToken);
            return result;
        }


        [HttpDelete("deleteComment")]
        [Authorize]

        public async Task<IActionResult> deleteComment(int idComment)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            var result = await _commentService.deleteComment(idComment, accessToken);
            return result;
        }

    }
}
