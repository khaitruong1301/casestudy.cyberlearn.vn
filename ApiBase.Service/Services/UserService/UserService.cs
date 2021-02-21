using ApiBase.Repository.Models;
using ApiBase.Repository.Repository;
using ApiBase.Service.Constants;
using ApiBase.Service.Helpers;
using ApiBase.Service.Infrastructure;
using ApiBase.Service.Utilities;
using ApiBase.Service.ViewModels;
using ApiBase.Service.ViewModels.Users;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using ApiBase.Service.Services.PriorityService;

namespace ApiBase.Service.Services.UserService
{

    public interface IUserService : IService<AppUser, UserViewModel>
    {
        Task<ResponseEntity> SignUpAsync(InfoUser modelVm);
        Task<ResponseEntity> Login(UserLogin modelVm);
        Task<ResponseEntity> SignInFacebookAsync(DangNhapFacebookViewModel modelVm);
        Task<UserJira> getUserByToken(string token);

        Task<ResponseEntity> RegisterUser(UserJiraModel modelVm);
        Task<ResponseEntity> editUser(UserJiraModelUpdate modelVm);
        Task<ResponseEntity> SignIn(UserJiraLogin modelVm);
        Task<ResponseEntity> getUser(string keyword="");
        Task<ResponseEntity> deleteUser(int id);
        Task<ResponseEntity> getUserByProjectId(int idProject=0);






    }
    public class UserService : ServiceBase<AppUser, UserViewModel>, IUserService
    {
        IUserRepository _userRepository;
        IRoleRepository _roleRepository;
        IUserTypeRepository _userTypeRepository;
        IUserType_RoleRepository _userType_RoleRepository;
        IUserJiraRepository _useJiraRepository;
        IProjectRepository _projectRepository;
        IProject_UserReponsitory _project_userRepository;
        ITask_UserRepository _taskUserRepository;
        ICommentRepository _commentRepository;


        private readonly IAppSettings _appSettings;

        public UserService(IProjectRepository projectRepository,IUserRepository userRepos, IRoleRepository roleRepos, IUserTypeRepository userTypeRepos, IUserType_RoleRepository userType_RoleRepos, IAppSettings appSettings,IUserJiraRepository usjira, IProject_UserReponsitory project_userRepository, ITask_UserRepository task_UserRepository, ICommentRepository commentRepository,
            IMapper mapper)
            : base(userRepos, mapper)
        {
            _userRepository = userRepos;
            _roleRepository = roleRepos;
            _userTypeRepository = userTypeRepos;
            _userType_RoleRepository = userType_RoleRepos;
            _appSettings = appSettings;
            _useJiraRepository = usjira;
            _projectRepository = projectRepository;
            _project_userRepository = project_userRepository;
            _taskUserRepository = task_UserRepository;
            _commentRepository = commentRepository;
        }

        public async Task<ResponseEntity> SignUpAsync(InfoUser modelVm)
        {
            try
            {
                AppUser entity = await _userRepository.GetSingleByConditionAsync("email",modelVm.email);
                if (entity != null) // Kiểm tra email đã được sử dụng bởi tài khoản khác chưa
                    return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, modelVm, MessageConstants.EMAIL_EXITST);

                entity = _mapper.Map<AppUser>(modelVm);
                entity.deleted = false;
                
                //entity.gender = ;
                //entity.Id = Guid.NewGuid().ToString();
                // Mã hóa mật khẩu
                //entity.MatKhau = BCrypt.Net.BCrypt.HashPassword(modelVm.MatKhau);
                entity.avatar = "/static/user-icon.png";
                entity.userTypeId = "CUSTOMER";

                entity = await _userRepository.InsertAsync(entity);
                if (entity == null)
                    return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, modelVm, MessageConstants.SIGNUP_ERROR);

                return new ResponseEntity(StatusCodeConstants.CREATED, modelVm, MessageConstants.SIGNUP_SUCCESS);
            }
            catch(Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, modelVm, MessageConstants.SIGNUP_ERROR);
            }
        }

        public async Task<ResponseEntity> Login(UserLogin modelVm)
        {
            try
            {
                // Lấy ra thông tin người dùng từ database dựa vào email
                AppUser entity = await _userRepository.GetSingleByConditionAsync("email",modelVm.email);
                if (entity == null)// Nếu email sai
                    return new ResponseEntity(StatusCodeConstants.NOT_FOUND, modelVm, MessageConstants.SIGNIN_WRONG);
                // Kiểm tra mật khẩu có khớp không
                //if (!BCrypt.Net.BCrypt.Verify(modelVm.MatKhau, entity.MatKhau))
                //    // Nếu password không khớp
                //    return new ResponseEntity(StatusCodeConstants.NOT_FOUND, modelVm, MessageConstants.SIGNIN_WRONG);

                List<KeyValuePair<string, dynamic>> columns = new List<KeyValuePair<string, dynamic>>();
                columns.Add(new KeyValuePair<string, dynamic>("email", modelVm.email));
                columns.Add(new KeyValuePair<string, dynamic>("password", modelVm.password));
                if (entity == null)// Nếu email sai
                    return new ResponseEntity(StatusCodeConstants.NOT_FOUND, modelVm, MessageConstants.SIGNIN_WRONG);
                entity = await _userRepository.GetSingleByListConditionAsync(columns);
                // Tạo token
                string token = await GenerateToken(entity);
                if (token == string.Empty)
                    return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, modelVm, MessageConstants.TOKEN_GENERATE_ERROR);
                UserLoginResult userLogin = new UserLoginResult();
                userLogin.email = modelVm.email;
                userLogin.accessToken = token;  
                return new ResponseEntity(StatusCodeConstants.OK, userLogin, MessageConstants.SIGNIN_SUCCESS);
            }
            catch(Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, modelVm, MessageConstants.SIGNIN_ERROR);
            }
        }

        private async Task<string> GenerateToken(AppUser entity)
        {
            try
            {
                UserType group = await _userTypeRepository.GetSingleByIdAsync( entity.userTypeId);
                if (group == null)
                    return string.Empty;

                IEnumerable<UserType_Role> group_Role = await _userType_RoleRepository.GetMultiByConditionAsync("userTypeId", group.id);

                List<string> lstDanhSachQuyen = new List<string>();
                foreach (var item in group_Role) {
                    lstDanhSachQuyen.Add(item.roleId);
                }

                string danhSachQuyen = JsonConvert.SerializeObject(lstDanhSachQuyen);
                List<string> roles = JsonConvert.DeserializeObject<List<string>>(danhSachQuyen);

                List<Claim> claims = new List<Claim>();
                //claims.Add(new Claim(ClaimTypes.Name, entity.Id));
                claims.Add(new Claim(ClaimTypes.Email, entity.email));
                if (roles != null)
                {
                    foreach (var item in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item.Trim()));
                    }
                }
                var secret = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var token = new JwtSecurityToken(
                        claims: claims,
                        notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                        expires: new DateTimeOffset(DateTime.Now.AddMinutes(60)).DateTime,
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
                    );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ResponseEntity> SignInFacebookAsync(DangNhapFacebookViewModel modelVm)
        {


            string[] ERR_MESSAGE = { "Vui lòng nhập email bạn đã đăng ký!", "Email này đã được sử dụng cho tài khoản facebook khác!", "Email không chính xác!" };
            string[] ERR_STATUS = { "EMAIL_ENTER", "EMAIL_EXISTS", "EMAIL_INCORRECT" };

            try
            {
                
                var httpClient = new HttpClient { BaseAddress = new Uri("https://graph.facebook.com/v2.9/") };
                var response = await httpClient.GetAsync($"me?access_token={modelVm.facebookToken}&fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale");
                if (!response.IsSuccessStatusCode)
                {
                    return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, "Login facebook failure !", "Please, try login again");
                }
                var result = await response.Content.ReadAsStringAsync();
                dynamic facebookAccount = JsonConvert.DeserializeObject<FacebookResult>(result);

                //Checkfacebook id
                UserJira facebookUser = await _useJiraRepository.GetSingleByConditionAsync("facebookId",facebookAccount.id);
                if (facebookUser != null)
                {
                    UserLoginResult userResult = new UserLoginResult();
                    userResult.email = facebookUser.email;
                    userResult.accessToken = await GenerateTokenJira(facebookUser);
                    return new ResponseEntity(StatusCodeConstants.OK, userResult, MessageConstants.SIGNIN_SUCCESS);

                }
                //Đăng nhập thành công fb kiểm tra có email không nếu có cho dn thành công
                Type objType = facebookAccount.GetType();
                if (objType.GetProperty("email") != null)
                {
                    //Kiểm tra có email chưa lấy ra
                    AppUser userCheckEmail = await _userRepository.GetSingleByConditionAsync("email", facebookAccount.email);
                    if (userCheckEmail != null)
                    {
                        //Cập nhật fb id cho mail đó
                        userCheckEmail.facebookId = facebookAccount.id;
                        await _userRepository.UpdateByConditionAsync("email", facebookAccount.email, userCheckEmail);
                        UserLoginResult userResult = new UserLoginResult();
                        userResult.email = userCheckEmail.email;
                        userResult.accessToken = await GenerateTokenJira(facebookUser);
                        return new ResponseEntity(StatusCodeConstants.OK, userResult, MessageConstants.SIGNIN_SUCCESS);
                    }
                }
                //Nếu chưa có tạo tài khoản
                AppUser userModel = new AppUser();
                userModel.facebookId = facebookAccount.id;
                userModel.name = facebookAccount.first_name + " " + facebookAccount.last_name;
                userModel.email = userModel.facebookId + "@facebook.com";
                userModel.deleted = false;
                userModel.avatar = "/static/user-icon.png";
                userModel.userTypeId = "CUSTOMER";

                AppUser userInsert = await _userRepository.InsertAsync(userModel);
                if (userInsert != null)
                {
                    UserLoginResult userResult = new UserLoginResult();
                    userResult.email = userModel.email;
                    userResult.accessToken = await GenerateToken(userModel);
                    return new ResponseEntity(StatusCodeConstants.OK, userResult, MessageConstants.SIGNIN_SUCCESS);
                }

                return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, ERR_STATUS[0], ERR_MESSAGE[0]);

            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, ERR_STATUS[0], ERR_MESSAGE[0]);
            }
                    //register if required
                    //var facebookUser = _context.FacebookUsers.SingleOrDefault(x => x.Id == facebookAccount.Id);
                    //if (facebookUser == null)
                    //{
                    //    var user = new ApplicationUser { UserName = facebookAccount.Name, Email = facebookAccount.Email };
                    //    var result2 = await _userManager.CreateAsync(user);
                    //    if (!result2.Succeeded) return BadRequest();
                    //    facebookUser = new FacebookUser { Id = facebookAccount.Id, UserId = user.Id };
                    //    _context.FacebookUsers.Add(facebookUser);
                    //    _context.SaveChanges();
                    //}


            //    }
            //    return new ResponseEntity(StatusCodeConstants.OK, result, MessageConstants.SIGNIN_SUCCESS);
            //}
            //catch (Exception ex)
            //{
            //    return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, ex.Message, MessageConstants.SIGNIN_ERROR);
            //}
            //string[] ERR_MESSAGE = { "Vui lòng nhập email bạn đã đăng ký!", "Email này đã được sử dụng cho tài khoản facebook khác!", "Email không chính xác!" };
            //string[] ERR_STATUS = { "EMAIL_ENTER", "EMAIL_EXISTS", "EMAIL_INCORRECT" };

            //try
            //{
            //    UserLoginResult result = new UserLoginResult();

            //    AppUser entity = await _userRepository.GetByFacebookAsync(modelVm.FacebookId);
            //    if (entity != null) // Nếu FacebookId đúng => đăng nhập thành công
            //    {
            //        // Tạo token
            //        result.accessToken = await GenerateToken(entity);
            //        result.email = entity.email;
            //        return new ResponseEntity(StatusCodeConstants.OK, result, MessageConstants.SIGNIN_SUCCESS);
            //    }

            //    //// Nếu facebook id sai và email chưa nhập
            //    //if (string.IsNullOrEmpty(modelVm.Email))
            //    //    return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, ERR_STATUS[0], ERR_MESSAGE[0]);


            //    if (entity == null)
            //    {
            //        var httpClient = new HttpClient { BaseAddress = new Uri("https://graph.facebook.com/v2.9/") };
            //        var response = await httpClient.GetAsync($"me?access_token={facebookToken.Token}&fields=id,name,email,first_name,last_name,age_range,birthday,gender,locale,picture");
            //        if (!response.IsSuccessStatusCode) return BadRequest();
            //        var result = await response.Content.ReadAsStringAsync();
            //        var facebookAccount = JsonConvert.DeserializeObject<FacebookAccount>(result);

            //        //register if required
            //        var facebookUser = _context.FacebookUsers.SingleOrDefault(x => x.Id == facebookAccount.Id);
            //        if (facebookUser == null)
            //        {
            //            var user = new ApplicationUser { UserName = facebookAccount.Name, Email = facebookAccount.Email };
            //            var result2 = await _userManager.CreateAsync(user);
            //            if (!result2.Succeeded) return BadRequest();
            //            facebookUser = new FacebookUser { Id = facebookAccount.Id, UserId = user.Id };
            //            _context.FacebookUsers.Add(facebookUser);
            //            _context.SaveChanges();
            //        }


            //    }
            //    return new ResponseEntity(StatusCodeConstants.OK, result, MessageConstants.SIGNIN_SUCCESS);
            //}
            //catch (Exception ex)
            //{
            //    return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, ex.Message, MessageConstants.SIGNIN_ERROR);
            //}
        }

        public async Task<UserJira> getUserByToken(string tokenString)
        {
            try
            {
                string email = FuncUtilities.parseJWTToEmail(tokenString);
                UserJira us = await _useJiraRepository.GetSingleByConditionAsync("email", email);
                return us;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ResponseEntity> RegisterUser(UserJiraModel modelVm)
        {
            try
            {
                UserJira entity = await _useJiraRepository.GetSingleByConditionAsync("email", modelVm.email);
                if (entity != null) // Kiểm tra email đã được sử dụng bởi tài khoản khác chưa
                    return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, modelVm, MessageConstants.EMAIL_EXITST);

                entity = _mapper.Map<UserJira>(modelVm);
                entity.avatar = "https://ui-avatars.com/api/?name=" + entity.name;
                entity.alias = FuncUtilities.BestLower(modelVm.name);
                entity.deleted = false;
                //entity.gender = ;
                //entity.Id = Guid.NewGuid().ToString();
                // Mã hóa mật khẩu
                //entity.MatKhau = BCrypt.Net.BCrypt.HashPassword(modelVm.MatKhau);
                //entity.avatar = "/static/user-icon.png";
                entity = await _useJiraRepository.InsertAsync(entity);


                if (entity == null)
                    return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, modelVm, MessageConstants.SIGNUP_ERROR);

                return new ResponseEntity(StatusCodeConstants.OK, modelVm, MessageConstants.SIGNUP_SUCCESS);
            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, modelVm, MessageConstants.SIGNUP_ERROR);
            }
        }

        private async Task<string> GenerateTokenJira(UserJira entity)
        {
            try
            {
                //UserType group = await _userTypeRepository.GetSingleByIdAsync(entity.userTypeId);
                //if (group == null)
                //    return string.Empty;

                //IEnumerable<UserType_Role> group_Role = await _userType_RoleRepository.GetMultiByConditionAsync("userTypeId", group.id);

                //List<string> lstDanhSachQuyen = new List<string>();
                //foreach (var item in group_Role)
                //{
                //    lstDanhSachQuyen.Add(item.roleId);
                //}

                //string danhSachQuyen = JsonConvert.SerializeObject(lstDanhSachQuyen);
                //List<string> roles = JsonConvert.DeserializeObject<List<string>>(danhSachQuyen);

                List<Claim> claims = new List<Claim>();
                //claims.Add(new Claim(ClaimTypes.Name, entity.Id));
                claims.Add(new Claim(ClaimTypes.Email, entity.email));
                //if (roles != null)
                //{
                //    foreach (var item in roles)
                //    {
                //        claims.Add(new Claim(ClaimTypes.Role, item.Trim()));
                //    }
                //}
                var secret = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var token = new JwtSecurityToken(
                        claims: claims,
                        notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                        expires: new DateTimeOffset(DateTime.Now.AddMinutes(60)).DateTime,
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
                    );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<ResponseEntity> SignIn(UserJiraLogin modelVm)
        {
            UserJira entity = await _useJiraRepository.GetSingleByConditionAsync("email", modelVm.email);
            if (entity == null) // Kiểm tra email đã được sử dụng bởi tài khoản khác chưa
                return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, "Email", "Email không tồn tại !");

            if(entity.passWord != modelVm.passWord)
            {
                return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, modelVm, "Tài khoản hoặc mật khẩu không đúng !");
            }
            UserJiraModelView usModel = new UserJiraModelView();
            usModel.id = entity.id;
            usModel.name = entity.name;
            usModel.avatar = entity.avatar;
            usModel.email = entity.email;
            usModel.phoneNumber = entity.phoneNumber;
            usModel.accessToken = await GenerateTokenJira(entity);
            return new ResponseEntity(StatusCodeConstants.OK, usModel, MessageConstants.MESSAGE_SUCCESS_200);
        }

        public async Task<ResponseEntity> getUser(string keyword = "")
        {
            IEnumerable<UserJira> entity =  _useJiraRepository.GetAllAsync().Result;
            List<Member> members = new List<Member>();
           
            if (entity.Count() != 0)
            {
                keyword = FuncUtilities.BestLower(keyword);
                List<UserJira> lstTimTheoTen = entity.Where(n => n.alias.Contains(keyword)).ToList();

                List<UserJira> lstTimTheoSdt = entity.Where(n => n.phoneNumber.Contains(keyword)).ToList();

                List<UserJira> lstTimTheoEmail = entity.Where(n => n.email.Contains(keyword)).ToList();

                List<UserJira> lstTimTheoMa = entity.Where(n => n.id.ToString().Contains(keyword)).ToList();

                IEnumerable<UserJira> result = new List<UserJira>();
                result = result.Union(lstTimTheoTen);
                result= result.Union(lstTimTheoSdt);
                result = result.Union(lstTimTheoEmail);
                result = result.Union(lstTimTheoMa);



                foreach (UserJira item in result)
                {
                    Member mem = new Member();

                    mem.userId = item.id;
                    mem.name = item.name;
                    mem.avatar = item.avatar;
                    mem.phoneNumber = item.phoneNumber;
                    mem.email = item.email;
                   
                    members.Add(mem);
                }
                return new ResponseEntity(StatusCodeConstants.OK, members, MessageConstants.MESSAGE_SUCCESS_200);

            }
            return new ResponseEntity(StatusCodeConstants.OK, members, MessageConstants.MESSAGE_SUCCESS_200);

        }

        public async Task<ResponseEntity> getUserByProjectId(int idProject = 0)
        {
            var project = _project_userRepository.GetMultiByConditionAsync("projectId", idProject).Result;

            if(project.Count() == 0)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, "User not found in the project!", MessageConstants.MESSAGE_ERROR_404);

            }

            List<Member> lstUser = new List<Member>();

            Member us;
            foreach (var item in project)
            {
                 us = new Member();
                var user = _useJiraRepository.GetSingleByConditionAsync("id", item.userId).Result;
                us.userId = user.id;
                us.name = user.name;
                us.avatar = user.avatar;
                lstUser.Add(us);
            }

            return new ResponseEntity(StatusCodeConstants.OK, lstUser, MessageConstants.MESSAGE_SUCCESS_200);


        }

        public async Task<ResponseEntity> editUser(UserJiraModelUpdate modelVm)
        {
            var userEdit = _useJiraRepository.GetSingleByConditionAsync("id", modelVm.id).Result;

            try
            {
                if (userEdit == null)
            {
                return new ResponseEntity(StatusCodeConstants.NOT_FOUND, MessageConstants.DELETE_ERROR, MessageConstants.MESSAGE_ERROR_404);

            }

            userEdit.name = modelVm.name;
                userEdit.email = modelVm.email;
            userEdit.passWord = modelVm.passWord;
                userEdit.phoneNumber = modelVm.phoneNumber;
                    userEdit.avatar = "https://ui-avatars.com/api/?name=" + userEdit.name;
                await _useJiraRepository.UpdateAsync(modelVm.id, userEdit);


                return new ResponseEntity(StatusCodeConstants.OK, MessageConstants.UPDATE_SUCCESS, MessageConstants.MESSAGE_SUCCESS_200);
            }
            catch (Exception err)
            {
                return new ResponseEntity(StatusCodeConstants.OK, MessageConstants.UPDATE_ERROR, MessageConstants.MESSAGE_ERROR_400);

            }


        }

        public async Task<ResponseEntity> deleteUser(int id)
        {
            try
            {
                var userEdit = _useJiraRepository.GetSingleByConditionAsync("id", id).Result;
                if(userEdit == null)
                {
                    return new ResponseEntity(StatusCodeConstants.NOT_FOUND, MessageConstants.DELETE_ERROR, MessageConstants.MESSAGE_ERROR_404);

                }
                List<KeyValuePair<string, dynamic>> columns = new List<KeyValuePair<string, dynamic>>();
                columns.Add(new KeyValuePair<string, dynamic>("userId", userEdit.id));
                var lstUserProject = _project_userRepository.GetMultiByListConditionAndAsync(columns).Result;



                var lstTask = _taskUserRepository.GetMultiByListConditionAndAsync(columns).Result;



                var lstComment = _taskUserRepository.GetMultiByListConditionAndAsync(columns).Result;

                List<KeyValuePair<string, dynamic>> columns1 = new List<KeyValuePair<string, dynamic>>();
                columns1.Add(new KeyValuePair<string, dynamic>("creator", userEdit.id));
                var project = _projectRepository.GetMultiByListConditionAndAsync(columns1).Result;

                if(project.Count()>0)
                {
                    return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, "Người dùng đã tạo project không thể xoá được !", MessageConstants.MESSAGE_ERROR_400);

                }


                List<dynamic> lstResult = new List<dynamic>();
                foreach (var item in lstTask)
                {
                    lstResult.Add(item.id);

                }
                await _taskUserRepository.DeleteByIdAsync(lstResult);

                List<dynamic> lstResult1 = new List<dynamic>();

                foreach (var item in lstUserProject)
                {
                    lstResult1.Add(item.id);
                }
                await _project_userRepository.DeleteByIdAsync(lstResult1);




                List<dynamic> lstResult2 = new List<dynamic>();

                foreach (var item in lstComment)
                {
                    lstResult2.Add(item.id);
                }
                await _commentRepository.DeleteByIdAsync(lstResult2);







                List<dynamic> lstId = new List<dynamic>();
                lstId.Add(id);





                await _useJiraRepository.DeleteByIdAsync(lstId);

                return new ResponseEntity(StatusCodeConstants.OK, MessageConstants.DELETE_SUCCESS, MessageConstants.MESSAGE_SUCCESS_200);
            }catch (Exception err)
            {
                return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, MessageConstants.DELETE_ERROR, MessageConstants.MESSAGE_ERROR_400);

            }

        }
    }
}
