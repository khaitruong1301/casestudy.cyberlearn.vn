using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiBase.Service.Helpers;
using ApiBase.Service.SignalR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ApiBase.Service.Services;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerUI;
using ApiBase.Api.Swagger;
using ApiBase.Service.Services.ProductManagementService;
using ApiBase.Repository.Repository;
using AutoMapper;
using ApiBase.Service.AutoMapper;
using ApiBase.Api.Filter;
using ApiBase.Service.Services.UserService;
using ApiBase.Service.Services.CommentService;
using ApiBase.Service.Services.PriorityService;
using ApiBase.Service.Services.Project_UserService;
using ApiBase.Service.Services.TaskTypeService;
using ApiBase.Service.Services.StatusService;

namespace ApiBase.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private readonly string CorsPolicy = "CorsPolicy";

        //public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // ========================== Product =============
            services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddSingleton<IProductManagementService, ProductManagementService>();
            //===================== ProjectCategory =====================
            services.AddSingleton<ICommentRepository, CommentRepository>();
            services.AddSingleton<ICommentService, CommentService>();
            //==============TaskType==========================
            services.AddSingleton<ITaskTypeRepository, TaskTypeRepository>();
            services.AddSingleton<ITaskTypeService, TaskTypeService>();

            //===================== Project_User ==============

            services.AddSingleton<IProject_UserReponsitory, Project_UserReponsitory>();
            services.AddSingleton<IProject_UserService, Project_UserService>();

            //===================== useJira =====================
            services.AddSingleton<IUserJiraRepository, UserJiraRepository>();

            services.AddSingleton<IUserService, UserService>();

            //===================== CommentCategory =====================
            services.AddSingleton<IProjectCategoryRepository, ProjectCategoryRepository>();
            services.AddSingleton<IProjectCategoryService, ProjectCategoryService>();

            //===================== Project =====================
            services.AddSingleton<IProjectRepository, ProjectRepository>();
            services.AddSingleton<IProjectService, ProjectService>();

            //===================== Status =====================
            services.AddSingleton<IStatusRepository, StatusRepository>();

            services.AddSingleton<IStatusService, StatusService>();
            //===================== Priority =====================

            services.AddSingleton<IPriorityService, PriorityService>();

            //===================== Priority =====================
            //services.AddSingleton<ITaskRepository, TaskRepository>();
            services.AddSingleton<ITaskRepository, TaskRepository>();

            //===================== Comment =====================
            services.AddSingleton<ICommentRepository, CommentRepository>();
            //===================== Task_User ================
            services.AddSingleton<ITask_UserRepository, Task_UserRepository>();
            //========================== Task ==================




            ////===================== ProjectCategory =====================
            //services.AddSingleton<IProjectCategoryRepository, ProjectCategoryRepository>();
            //services.AddSingleton<IProjectCategoryService, ProjectCategoryService>();
            ////===================== CommentCategory =====================
            //services.AddSingleton<IProjectCategoryRepository, ProjectCategoryRepository>();
            //services.AddSingleton<IProjectCategoryService, ProjectCategoryService>();


            //===================== Priority ============================

            services.AddSingleton<IPriorityRepository, PriorityRepository>();
            services.AddSingleton<IPriorityService, PriorityService>();

            // ========================== User ================
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<IUserService, UserService>();
            //services.AddSingleton<IUserService, UserService>();
            // ========================== Authorize ===========
            services.AddSingleton<IUserTypeRepository, UserTypeRepository>();
            services.AddSingleton<IRoleRepository, RoleRepository>();
            services.AddSingleton<IUserType_RoleRepository, UserType_RoleRepository>();

            // ==================== HELPER ====================
            services.AddSingleton<IFacebookService, FacebookService>();
            services.AddSingleton<IMailService, MailService>();
            services.AddSingleton<INotificationService, NotificationService>();
            services.AddSingleton<ITranslator, Translator>();

            // ==================== SWAGGER ====================
            services.ConfigureSwaggerGen(options =>
            {
                options.OperationFilter<AuthorizationHeaderParameterOperationFilter>();
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "API", Version = "v1" });
            });

            

            // ==================== CORS ORIGIN ====================
            services.AddCors(
                options => options.AddPolicy(CorsPolicy,
                builder => {
                    builder.WithOrigins("http://localhost:3000", "http://crm.myclass.vn", "https://login.cybersoft.edu.vn", "https://crm.cybersoft.edu.vn", "*")
                           .AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .AllowCredentials()
                           .Build();
                }));

            // ==================== SIGNALR ====================
            services.AddSignalR();

            // ==================== SECTION CONFIG ====================
            //froservices.AddSingleton<IFacebookSettings>(
            //    Configuration.GetSection("FacebookSettings").Get<FacebookSettings>());
            services.AddSingleton<IMailSettings>(
                  Configuration.GetSection("MailSettings").Get<MailSettings>());
            services.AddSingleton<IFtpSettings>(
                Configuration.GetSection("FtpSettings").Get<FtpSettings>());
            services.AddSingleton<ICaptchaSettings>(
                Configuration.GetSection("CaptchaSettings").Get<CaptchaSettings>());

            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.AddSingleton<IAppSettings>(
                Configuration.GetSection("AppSettings").Get<AppSettings>());


            // ==================== FACEBOOK LOGIN ====================
            //services.AddAuthentication().AddFacebook(facebookOptions =>
            //{
            //    facebookOptions.AppId = Configuration["Authentication:Facebook:AppId"];
            //    facebookOptions.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
            //});

            // ==================== JWT AUTHENTICATION CONFIG ====================
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(x =>
            {
                // Đặt tiền tố cho header token (Sử dụng mặc định là Bearer)
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false; // Cấu hình không bắt buộc sử dụng https
                //Lưu bearer token trong Microsoft.AspNetCore.Http.Authentication.AuthenticationProperties
                x.SaveToken = true; // Sau khi đăng nhập thành công
                // Set or get các tham số lưu vào token
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true, // Bắt buộc phải có SigningKey
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false, // Issuer không bắt buộc
                    ValidateAudience = false, // Audience không bắt buộc
                    ValidateLifetime = true, // Thời gian hết hạn (expires) là bắt buộc
                    ClockSkew = TimeSpan.FromDays(20)
                };
                x.IncludeErrorDetails = true;
                x.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = c => //
                    {
                        c.NoResult();
                        c.Response.StatusCode = 401;
                        c.Response.ContentType = "text/plain";
                        return c.Response.WriteAsync(c.Exception.ToString());
                    }
                };
            });

            // ==================== AUTO MAPPER ====================
            services.AddSingleton(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new EntityToViewModelProfile());
                cfg.AddProfile(new ViewModelToEntityProfile());
            }).CreateMapper());

            services.AddMvc(opt => {
                opt.Filters.Add(typeof(ValidateModelFilter));
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // ==================== HANDLER EXCEPTION ====================
            //app.UseExceptionHadler();

            // ==================== CORS ORIGIN ====================
            app.UseCors(CorsPolicy);

            // ==================== SIGNALR ====================
            //app.UseSignalR(routes =>
            //{
            //    routes.MapHub<AppHub>("/apphub");
            //});

            // ==================== AUTHEN JWT ====================
            app.UseAuthentication();

            app.UseHttpsRedirection();

            //khai bao su dung  quyen folder hinh
            app.UseStaticFiles();
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot")),
                RequestPath = new PathString("/wwwroot"),

            });

            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "images")),
            //    RequestPath = new PathString("/images"),

            //});
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot", "files")),
            //    RequestPath = new PathString("/files"),

            //});
            //app.UseStaticFiles(new StaticFileOptions()
            //{
            //    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot","cmnd")),
            //    RequestPath = new PathString("/cmnd"),

            //});

            app.UseStaticFiles();



            app.UseMvc();

            // ==================== SWAGGER ====================
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SOLO DEV API VERSION 01");
                c.DocumentTitle = "API";
                //c.DocExpansion(DocExpansion.None); TAT DEMO

            });
        }
    }
}
