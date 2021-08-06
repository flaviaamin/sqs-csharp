using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ReCall2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            var queue = "CREATE_MESSAGE";
            var pathFile = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
            if (File.Exists(pathFile))
            {
                JToken jAppSettings = JToken.Parse(System.IO.File.ReadAllText(pathFile));
                ConfigManager.AwsId = jAppSettings["awsId"].ToString();
                ConfigManager.AwsKey = jAppSettings["awsKey"].ToString();
                ConfigManager.SqsHost = jAppSettings["queue"][queue]["sqsHost"].ToString();
                ConfigManager.SqsId = jAppSettings["queue"][queue]["sqsId"].ToString();
                ConfigManager.SqsName = jAppSettings["queue"][queue]["sqsName"].ToString();
            }
            else
            {
                ConfigManager.AwsId = Environment.GetEnvironmentVariable("AWS_ID");
                ConfigManager.AwsKey = Environment.GetEnvironmentVariable("AWS_KEY");
                ConfigManager.SqsHost = Environment.GetEnvironmentVariable($"{queue}_SQS_HOST");
                ConfigManager.SqsId = Environment.GetEnvironmentVariable($"{queue}_SQS_ID");
                ConfigManager.SqsName = Environment.GetEnvironmentVariable($"{queue}_SQS_NAME");
            }

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

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
