﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchoolManagement.Application.Students;
using SchoolManagement.CustomerMiddlewares;
using SchoolManagement.DataRepositories;
using SchoolManagement.Infrastructure;
using SchoolManagement.Infrastructure.Repositories;
using SchoolManagement.Models;
using SchoolManagement.Security;
using System;

namespace SchoolManagement
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("DefaultDbConnection"), MySqlServerVersion.LatestSupportedServerVersion);
            });

            services
                .AddControllersWithViews(configure =>
                {
                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                    configure.Filters.Add(new AuthorizeFilter(policy));
                })
                .AddRazorRuntimeCompilation()
                .AddXmlSerializerFormatters();

            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddTransient(typeof(IRepository<,>), typeof(RepositoryBase<,>));

            services.AddScoped<IStudentService, StudentService>();

            services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
            {
                microsoftOptions.ClientId = Configuration["Authentication:Microsoft:ClientId"];
                microsoftOptions.ClientSecret = Configuration["Authentication:Microsoft:ClientSecret"];
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;    // 密码最小长度验证
                options.Password.RequiredUniqueChars = 3;   // 密码中允许最大的重复字符数
                options.Password.RequireNonAlphanumeric = false;    // 密码中至少有一个非字母数字的字符
                options.Password.RequireLowercase = false;  // 密码是否必须包含小写字母
                options.Password.RequireUppercase = false;  // 密码是否必须包含大写字母
                options.Password.RequireDigit = true;  // 密码是否必须包含数字

                // options.SignIn.RequireConfirmedEmail = true;    // 电子邮箱的验证
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddHttpContextAccessor();

            services.AddAuthorization(options =>
            {
                // 角色是Admin且声明包含Edit Role值为true 或者 角色是SuperManager
                //options.AddPolicy("EditRolePolicy", policy => policy.RequireAssertion(context =>
                //    context.User.IsInRole("Admin") &&
                //    context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
                //    context.User.IsInRole("SuperManager")
                //));

                options.AddPolicy("EditRolePolicy", policy => policy.AddRequirements(new ManageAdminRolesAndClaimRequirement()));

                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));

                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));

                options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("Admin", "User", "SuperManager"));   // 策略结合多个角色进行授权

                // options.InvokeHandlersAfterFailure = false;
            });

            services.AddDataProtection();

            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();

            services.ConfigureApplicationCookie(options =>
            {
                // 修改拒绝访问的路由地址
                options.AccessDeniedPath = new PathString("/Admin/AccessDenied");

                // 统一系统全局的Cookie名称
                options.Cookie.Name = "SchoolManagementCookie";

                // 登录用户Cookie的有效期
                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);

                // 是否对Cookie启用滑动过期时间
                options.SlidingExpiration = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else if (env.IsStaging() || env.IsProduction() || env.IsEnvironment("UAT"))
            {
                app.UseExceptionHandler("/Error");
                app.UseStatusCodePagesWithReExecute("/Error/{0}");  //{0}占位符，会自动接收http中的状态码
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
