using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvcCore()
            .AddAuthorization()
            .AddJsonFormatters();

            //Adds the authentication services to DI and configures "Bearer" as the default scheme
            string identityServerUrl = Environment.GetEnvironmentVariable("identity_server_url");
            Console.WriteLine("\n00 - Current Configuration:");
            Console.WriteLine("- Identity Server URL (identity_server_url): " + identityServerUrl);
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {                    
                    options.Authority = identityServerUrl; //IdentityServer Address
                    options.RequireHttpsMetadata = false;
                    options.Audience = "api1";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //Adds the authentication middleware to the pipeline so authentication will be performed automatically on every call into the host
            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
