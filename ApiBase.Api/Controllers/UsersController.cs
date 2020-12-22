using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiBase.Repository.Models;
using ApiBase.Repository.Repository;
using ApiBase.Service.Constants;
using ApiBase.Service.Services.UserService;
using ApiBase.Service.ViewModels;
using ApiBase.Service.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiBase.Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        IUserRepository _userRepository;
        IUserService _userService;


        public UsersController(IUserRepository userRepository,IUserService userService)
        {
            _userRepository = userRepository;
            _userService = userService;
        }


        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] UserJiraModel model)
        {
            return await _userService.RegisterUser(model);
        }

        [HttpGet("getUser")]
        [Authorize]
        public async Task<IActionResult> getUser(string keyword = "") {



            return await _userService.getUser(keyword);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] UserJiraLogin model)
        {
            return await _userService.SignIn(model);
        }

        //[HttpPost("signup")]
        //public async Task<IActionResult> SignUp([FromBody] InfoUser model)
        //{
        //    return await _userService.SignUpAsync(model);
        //}


        //[HttpPost("signin")]
        //public async Task<IActionResult> Signin([FromBody] UserLogin model)
        //{
        //    return await _userService.Login(model);
        //}


        [HttpPost("facebooklogin")]
        public async Task<IActionResult> FacebookLogin([FromBody] DangNhapFacebookViewModel model)
        {
            return await _userService.SignInFacebookAsync(model);
        }



        [HttpPost("TestToken")]
        [Authorize]
        public async Task<IActionResult> TestToken()
        {

            return  new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "Okay ! đã check token được phép truy cập !", MessageConstants.SIGNIN_SUCCESS);
        }

        //[HttpPost("searchUserProject")]
        //public async Task<IActionResult> searchUserProject (int idProject=0)
        //{


        //    return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "Okay ! đã check token được phép truy cập !", MessageConstants.SIGNIN_SUCCESS);
        //}
    }
}
