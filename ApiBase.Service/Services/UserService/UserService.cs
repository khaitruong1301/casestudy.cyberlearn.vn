using ApiBase.Repository.Models;
using ApiBase.Repository.Repository;
using ApiBase.Service.Constants;
using ApiBase.Service.Helpers;
using ApiBase.Service.Infrastructure;
using ApiBase.Service.Utilities;
using ApiBase.Service.ViewModels;
using ApiBase.Service.ViewModels.Order;
using ApiBase.Service.ViewModels.Users;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ApiBase.Service.Services.UserService
{

    public interface IUserService : IService<AppUser, UserViewModel>
    {
        Task<ResponseEntity> SignUpAsync(InfoUser modelVm);
        Task<ResponseEntity> Login(UserLogin modelVm);
        Task<ResponseEntity> SignInFacebookAsync(DangNhapFacebookViewModel modelVm);

        Task<ResponseEntity> Order(OrderViewModel order);

        Task<ResponseEntity> DeleteOrder(string token, int idOrder);

        Task<ResponseEntity> OrderApproval(string token, int idOrder);

        Task<ResponseEntity> UpdateProfile(string token, InfoUser model);
        Task<ResponseEntity> ChangePassword(string token, UserNewPassword model);
        Task<ResponseEntity> GetProfile(string token);

        Task<ResponseEntity> like(string accesstoken, int productId);
        Task<ResponseEntity> unlike(string accesstoken, int productId);    
        Task<ResponseEntity> getProductFavorite(string accesstoken);


        




    }
    public class UserService : ServiceBase<AppUser, UserViewModel>, IUserService
    {
        IUserRepository _userRepository;
        IRoleRepository _roleRepository;
        IUserTypeRepository _userTypeRepository;
        IProductRepository _productRepository;
        IUserType_RoleRepository _userType_RoleRepository;
        IOrderProductRepository _orderRepository;
        private readonly IAppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;
        string myHostUrlImage = "";

        public UserService(IUserRepository userRepos, IRoleRepository roleRepos, IUserTypeRepository userTypeRepos, IUserType_RoleRepository userType_RoleRepos, IAppSettings appSettings, IOrderProductRepository orderRepository, IProductRepository productRepository, IHttpContextAccessor httpContextAccessor,
        IMapper mapper)
            : base(userRepos, mapper)
        {
            _userRepository = userRepos;
            _roleRepository = roleRepos;
            _userTypeRepository = userTypeRepos;
            _userType_RoleRepository = userType_RoleRepos;
            _appSettings = appSettings;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _httpContextAccessor = httpContextAccessor;
            myHostUrlImage = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/images/";


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
                entity.favoriteProducts = "[]";
                //entity.gender = ;
                //entity.Id = Guid.NewGuid().ToString();
                // Mã hóa mật khẩu
                //entity.MatKhau = BCrypt.Net.BCrypt.HashPassword(modelVm.MatKhau);
                entity.avatar = "user-icon.png";
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
                AppUser facebookUser = await _userRepository.GetByFacebookAsync(facebookAccount.id);
                if (facebookUser != null)
                {
                    UserLoginResult userResult = new UserLoginResult();
                    userResult.email = facebookUser.email;
                    userResult.accessToken = await GenerateToken(facebookUser);
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
                        userResult.accessToken = await GenerateToken(facebookUser);
                        return new ResponseEntity(StatusCodeConstants.OK, userResult, MessageConstants.SIGNIN_SUCCESS);
                    }
                }
                //Nếu chưa có tạo tài khoản
                AppUser userModel = new AppUser();
                userModel.facebookId = facebookAccount.id;
                userModel.name = facebookAccount.first_name + " " + facebookAccount.last_name;
                userModel.email = userModel.facebookId + "@facebook.com";
                userModel.deleted = false;
                userModel.avatar = "user-icon.png";
                userModel.userTypeId = "CUSTOMER";
                userModel.password = "Cybersoft@123";
                userModel.favoriteProducts = "[]";
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

        public async Task<ResponseEntity> Order(OrderViewModel orderVM)
        {
            try
            {

                if(orderVM.orderDetail.Count > 0)
                {
                    foreach(var item in orderVM.orderDetail)
                    {
                        Product p = await _productRepository.GetSingleByIdAsync(item.productId);
                        if(p == null)
                        {
                            return new ResponseEntity(StatusCodeConstants.OK, "Mã sản phẩm không hợp lệ!", MessageConstants.MESSAGE_ERROR_500);
                        }
                    }
                }

                string orderDetail = JsonConvert.SerializeObject(orderVM.orderDetail);

                OrderProduct model = new OrderProduct();
                model.date = FuncUtilities.GetDateTimeCurrent();
                model.deleted = false;
                model.email = orderVM.email;
                model.status = OrderStatus.CHUA_GIAO;
                model.orderDetail = orderDetail;
                var orederResult = await _orderRepository.InsertAsync(model);
                return new ResponseEntity(StatusCodeConstants.OK, MessageConstants.INSERT_SUCCESS, MessageConstants.MESSAGE_SUCCESS_200);
            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, ex.Message, MessageConstants.MESSAGE_ERROR_500);
            }

        }

        public async Task<ResponseEntity> DeleteOrder(string tokenstring, int idOrder)
        {
            try
            {
                var email = parseJWTToEmail(tokenstring);

                List<KeyValuePair<string, dynamic>> columns = new List<KeyValuePair<string, dynamic>>();
                columns.Add(new KeyValuePair<string, dynamic>("email", email));
                columns.Add(new KeyValuePair<string, dynamic>("id", idOrder));

                OrderProduct orderProduct = await _orderRepository.GetSingleByListConditionAsync(columns);
                if (orderProduct == null)
                {
                    return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "Yêu cầu hủy đơn không hợp lệ! Bạn không thể hủy đơn hàng của người khác !");

                }
                orderProduct.deleted = true;

                await _orderRepository.UpdateAsync(orderProduct.id, orderProduct);

                return new ResponseEntity(StatusCodeConstants.OK, MessageConstants.MESSAGE_SUCCESS_200);
            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, ex.Message, MessageConstants.MESSAGE_ERROR_500);
            }

        }

        public async Task<ResponseEntity> OrderApproval(string tokenstring, int idOrder)
        {

            try
            {
                List<KeyValuePair<string, dynamic>> columns = new List<KeyValuePair<string, dynamic>>();
                //columns.Add(new KeyValuePair<string, dynamic>("email", email));
                //columns.Add(new KeyValuePair<string, dynamic>("id", idOrder));

                OrderProduct orderProduct = await _orderRepository.GetSingleByConditionAsync("id", idOrder);
                if (orderProduct == null)
                {
                    return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, "Không tìm thấy đơn hàng !");

                }
                orderProduct.status = OrderStatus.DA_GIAO;


                await _orderRepository.UpdateAsync(orderProduct.id, orderProduct);

                //Lấy mảng product
                List<OrderDetail> orderDetail = JsonConvert.DeserializeObject<List<OrderDetail>>(orderProduct.orderDetail);
                foreach(var ctdh in orderDetail)
                {
                    Product pro = await _productRepository.GetSingleByIdAsync(ctdh.productId);

                    pro.quantity = pro.quantity - Convert.ToInt32(ctdh.quantity);

                    await _productRepository.UpdateAsync(pro.id, pro);
                }


                return new ResponseEntity(StatusCodeConstants.OK, MessageConstants.MESSAGE_SUCCESS_200);
            }catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, ex.Message, MessageConstants.MESSAGE_ERROR_500);
            }
        }
        
        public string parseJWTToEmail(string tokenstring)
        {
            
            tokenstring = tokenstring.Replace("Bearer ", "");
            //var stream = Encoding.ASCII.GetBytes("CYBERSOFT2020_SECRET");
            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenstring);
            var email = token.Claims.First(c => c.Type == ClaimTypes.Email).Value;
            return email;
        }
        public async Task<ResponseEntity> UpdateProfile(string token, InfoUser model)
        {
            try
            {
                var email = parseJWTToEmail(token);

                AppUser user = await _userRepository.GetSingleByConditionAsync("email", email);

                if (user == null)
                {
                    return new ResponseEntity(StatusCodeConstants.ERROR_SERVER,MessageConstants.MESSAGE_ERROR_400, MessageConstants.MESSAGE_ERROR_500);
                }
                if (email != model.email)
                { 
                    return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "Không thể cập nhật tài khoản khác!", MessageConstants.MESSAGE_ERROR_500);

                }
                //     public string name { get; set; }
                //public bool gender { get; set; }
                //public string phone { get; set; }
                user.name = model.name;
                user.gender = model.gender;
                user.phone = model.phone;

                await _userRepository.UpdateByConditionAsync("email", email, user);
                return new ResponseEntity(StatusCodeConstants.OK, MessageConstants.MESSAGE_SUCCESS_200);

            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, ex.Message, MessageConstants.MESSAGE_ERROR_500);
            }

        }

        public async Task<ResponseEntity> ChangePassword(string token, UserNewPassword model)
        {
            try
            {
                var email = parseJWTToEmail(token);

                AppUser user = await _userRepository.GetSingleByConditionAsync("email", email);

                if (user == null)
                {
                    return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, MessageConstants.MESSAGE_ERROR_500);
                }
                //if (email != model.email)
                //{
                //    return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "Không thể đổi mật khẩu người khác!", MessageConstants.MESSAGE_ERROR_500);

                //}
                //     public string name { get; set; }
                //public bool gender { get; set; }
                //public string phone { get; set; }
                user.password = model.newPassword;
                

                await _userRepository.UpdateByConditionAsync("email", email, user);
                return new ResponseEntity(StatusCodeConstants.OK, MessageConstants.MESSAGE_SUCCESS_200);

            }
            catch (Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, ex.Message, MessageConstants.MESSAGE_ERROR_500);
            }
        }

        [HttpGet("getprofile")]
        public async Task<ResponseEntity> GetProfile(string token)
        {
            try
            {
                var email = parseJWTToEmail(token);

                AppUser user = await _userRepository.GetSingleByConditionAsync("email", email);

                if (user == null)
                {
                    return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, MessageConstants.MESSAGE_ERROR_500);
                }
                AppUser app = await _userRepository.GetSingleByConditionAsync("email", email);
                userProfile usProfile = new userProfile();
                usProfile.avatar = app.avatar;
                usProfile.deleted = app.deleted;
                usProfile.email = app.email;
                usProfile.facebookId = app.facebookId;
                usProfile.gender = app.gender;
                usProfile.name = app.name;
                usProfile.phone = app.phone;
                usProfile.avatar = myHostUrlImage + app.avatar;


                //Lấy đơn hàng 
                var lstOrderHistory = await _orderRepository.GetMultiByConditionAsync("email", usProfile.email);
                if (lstOrderHistory.Count() > 0)
                {
                    foreach (var item in lstOrderHistory)
                    {
                        OrderHistory donHang = new OrderHistory();
                        donHang.alias = item.alias;
                        donHang.date = item.date;
                        donHang.email = item.email;
                        donHang.id = item.id;

                        List<OrderDetail> lstOrderDetail = JsonConvert.DeserializeObject<List<OrderDetail>>(item.orderDetail);
                        foreach (var ctdh in lstOrderDetail)
                        {
                            Product pro = await _productRepository.GetSingleByIdAsync(ctdh.productId);
                            OrderDetailHistory orHis = new OrderDetailHistory();
                            orHis.alias = pro.alias;
                            orHis.description = pro.description;
                            orHis.image = myHostUrlImage + pro.image;
                            orHis.name = pro.name;
                            orHis.shortDescription = pro.shortDescription;
                            donHang.orderDetail.Add(orHis);
                        }
                        usProfile.ordersHistory.Add(donHang);
                    }
                }
                return new ResponseEntity(StatusCodeConstants.OK,usProfile, MessageConstants.MESSAGE_SUCCESS_200);
            }
            catch(Exception ex)
            {
                return new ResponseEntity(StatusCodeConstants.BAD_REQUEST, ex.Message, MessageConstants.MESSAGE_ERROR_500);

            }

        }

        public async Task<ResponseEntity> like(string accesstoken, int productId)
        {
            var email = parseJWTToEmail(accesstoken);

            AppUser user = await _userRepository.GetSingleByConditionAsync("email", email);

            if (user == null)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "user is not found !");
            }

            var product = await _productRepository.GetSingleByIdAsync(productId);
            if (product == null)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "product is not found !");

            }

            UserFavorite usResult = new UserFavorite();
            usResult.email = email;


            usResult.ProductsFavorite = JsonConvert.DeserializeObject<List<ProductFavorite>>(user.favoriteProducts);
            int index = usResult.ProductsFavorite.FindIndex(n => n.id == productId);
            if (index == -1)
            {
                usResult.ProductsFavorite.Add(new ProductFavorite() { id = product.id, image = myHostUrlImage + product.image, name = product.name });
            }

            user.favoriteProducts = JsonConvert.SerializeObject(usResult.ProductsFavorite);

            await _userRepository.UpdateByConditionAsync("email", user.email, user);

            return new ResponseEntity(StatusCodeConstants.OK, "like is successfully!", MessageConstants.MESSAGE_SUCCESS_200);
        }

        public async Task<ResponseEntity> unlike(string accesstoken, int productId)
        {
            var email = parseJWTToEmail(accesstoken);

            AppUser user = await _userRepository.GetSingleByConditionAsync("email", email);

            if (user == null)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "user is not found !");
            }

            var product = await _productRepository.GetSingleByIdAsync(productId);
            if (product == null)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "product is not found !");
            }

            UserFavorite usResult = new UserFavorite();
            usResult.email = email;


            usResult.ProductsFavorite = JsonConvert.DeserializeObject<List<ProductFavorite>>(user.favoriteProducts);
            int index = usResult.ProductsFavorite.FindIndex(n => n.id == productId);
            if (index != -1)
            {
                usResult.ProductsFavorite.RemoveAt(index);
            }

            user.favoriteProducts = JsonConvert.SerializeObject(usResult.ProductsFavorite);

            await _userRepository.UpdateByConditionAsync("email", user.email, user);

            return new ResponseEntity(StatusCodeConstants.OK, "unlike is successfully!", MessageConstants.MESSAGE_SUCCESS_200);
        }

        public async Task<ResponseEntity> getProductFavorite(string accesstoken)
        {
            var email = parseJWTToEmail(accesstoken);

            AppUser user = await _userRepository.GetSingleByConditionAsync("email", email);

            if (user == null)
            {
                return new ResponseEntity(StatusCodeConstants.ERROR_SERVER, "user is not found !");
            }

            UserFavorite us = new UserFavorite();
            us.email = user.email;
            us.ProductsFavorite = JsonConvert.DeserializeObject<List<ProductFavorite>>(user.favoriteProducts);
           
                
            return new ResponseEntity(StatusCodeConstants.OK, us, MessageConstants.MESSAGE_SUCCESS_200);
        }
    }
}
