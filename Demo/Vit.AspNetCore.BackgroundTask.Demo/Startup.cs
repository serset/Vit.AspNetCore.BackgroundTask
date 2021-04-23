
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vit.Extensions;

namespace App
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
 
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);


            //(x.1)Startup.cs ���ö�ʱ����
            //services.AddHostedService<Vit.AspNetCore.BackgroundTask.BackgroundTaskService>();
            services.UseBackgroundTask();

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


            app.UseMvc();
            app.UseMvcWithDefaultRoute();


        }

    }
}