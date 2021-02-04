using Encoo.LowCode.WechatServer.Metadata;
using Encoo.LowCode.WechatServer.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Refit;

namespace Encoo.LowCode.WechatServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddControllersWithViews();
            services.AddRefitClient<IWechatApi>().ConfigureHttpClient(c=> { c.BaseAddress = new System.Uri("https://qyapi.weixin.qq.com"); });
            services.AddSingleton<CacheService>();
            services.AddDbContext<WechatServerDataContext>(optionbuilder =>
            {
                optionbuilder.UseSqlServer("Data Source=150.158.108.45;Initial Catalog=WechatTest;User ID=sa;password=Cugb0972Ftd;Integrated Security=false;");
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseMiddleware(typeof(CustomerExceptionMiddleware));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=WechatRedirect}/{action=PreAuth}");
                endpoints.MapControllers();
            });
        }
    }
}
