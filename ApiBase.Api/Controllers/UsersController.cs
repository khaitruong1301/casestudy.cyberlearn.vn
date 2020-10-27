using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiBase.Repository.Repository;
using ApiBase.Service.Constants;
using ApiBase.Service.Services;
using ApiBase.Service.Services.UserService;
using ApiBase.Service.Utilities;
using ApiBase.Service.ViewModels;
using ApiBase.Service.ViewModels.Order;
using ApiBase.Service.ViewModels.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApiBase.Api.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        IUserRepository _userRepository;
        IUserService _userService;
        IFileService _fileService;
       

        public UsersController(IUserRepository userRepository,IUserService userService, IFileService fileSV)
        {
            _userRepository = userRepository;
            _userService = userService;
            _fileService = fileSV;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] InfoUser model)
        {
            return await _userService.SignUpAsync(model);
        }


        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromBody] UserLogin model)
        {
            return await _userService.Login(model);
        }


        [HttpPost("facebooklogin")]
        public async Task<IActionResult> FacebookLogin([FromBody] DangNhapFacebookViewModel model)
        {
            return await _userService.SignInFacebookAsync(model);
        }

        [Authorize]
        [HttpPost("getProfile")]
        public async Task<IActionResult> getProfile()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];
            return await _userService.GetProfile(accessToken);
        }

        [Authorize]
        [HttpPost("updateProfile")]
        public async Task<IActionResult> updateProfile([FromBody] InfoUser model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];

            return await _userService.UpdateProfile(accessToken,model);
        }
        [Authorize]
        [HttpPost("changePassword")]
        public async Task<IActionResult> changePassword([FromBody] UserNewPassword model)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];

            return await _userService.ChangePassword(accessToken,model);
        }




        [Authorize]
        [HttpPost("deleteOrder")]
        public async Task<IActionResult> deleteOrder([FromBody] OrderDestroy orderDestroy)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];

            return await _userService.DeleteOrder(accessToken, orderDestroy.orderId);
        }

        [Authorize(Roles = "APPROVE")]
        [HttpPost("OrderApproval")]
        public async Task<IActionResult> orderApproval([FromBody] OrderDestroy orderApproval)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];

            return await _userService.OrderApproval(accessToken, orderApproval.orderId);
        }



        [HttpPost("order")]
        public async Task<IActionResult> Order([FromBody] OrderViewModel model)
        {
            return await _userService.Order(model);
        }
        //[Authorize]
        //[HttpPost("getinfo")]
        //public async Task<IActionResult> GetInfo([FromBody] DangNhapFacebookViewModel model)
        //{
        //    return await _userService.SignInFacebookAsync(model);
        //}
        [HttpPost("uploadavatar")]
        public async Task<IActionResult> UploadAvatar()
        {
            try
            {
                var frmData = Request.Form;
                IFormFileCollection files = null;
                UserUploadAvatar model = new UserUploadAvatar();

                if (frmData.Files.Count == 1)
                {
                    files = Request.Form.Files;
                    model.avatar = model.email + "-" + FuncUtilities.BestLower(model.email) + "." + files[0].FileName.Split(".")[files[0].FileName.Split('.').Length - 1];
                    await _fileService.UploadFileAsync(files, model.avatar);
                }
                return new ResponseEntity(StatusCodeConstants.OK, "Uploadfile thành công!", MessageConstants.MESSAGE_SUCCESS_200);

            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, ex.Message, MessageConstants.MESSAGE_ERROR_500);

            }


        }
        [HttpGet("like")]
        [Authorize]
        public async Task<IActionResult> like(int productId)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];

            return await _userService.like(accessToken,productId);
        }

        [HttpGet("unlike")]
        [Authorize]
        public async Task<IActionResult> unLike(int productId)
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];

            return await _userService.unlike(accessToken, productId);
        }
        [HttpGet("getproductfavorite")]
        [Authorize]
        public async Task<IActionResult> getProductFavorite()
        {
            var accessToken = Request.Headers[HeaderNames.Authorization];

            return await _userService.getProductFavorite(accessToken);
        }


    }
}
