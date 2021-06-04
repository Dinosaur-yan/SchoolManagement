using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SchoolManagement.CustomerMiddlewares;
using SchoolManagement.DataRepositories;
using SchoolManagement.Infrastructure;

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

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 6;    // 密码最小长度验证
                options.Password.RequiredUniqueChars = 3;   // 密码中允许最大的重复字符数
                options.Password.RequireNonAlphanumeric = false;    // 密码中至少有一个非字母数字的字符
                options.Password.RequireLowercase = false;  // 密码是否必须包含小写字母
                options.Password.RequireUppercase = false;  // 密码是否必须包含大写字母
                options.Password.RequireDigit = true;  // 密码是否必须包含数字
            });

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddEntityFrameworkStores<AppDbContext>();
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
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
