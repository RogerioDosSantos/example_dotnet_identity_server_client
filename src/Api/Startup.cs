using System;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Api
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string proxyUrl = Environment.GetEnvironmentVariable("proxy_url");
            string identityServerUrl = Environment.GetEnvironmentVariable("identity_server_url");
            string identityServerAudience = Environment.GetEnvironmentVariable("identity_server_audience");
            Console.WriteLine("\n00 - Current Configuration:");
            Console.WriteLine("- Proxy URL (proxy_url): " + proxyUrl);
            Console.WriteLine("- Identity Server URL (identity_server_url): " + identityServerUrl);
            Console.WriteLine("- Identity Server Audience (identity_server_audience): " + identityServerAudience);

            services.AddMvcCore()
            .AddAuthorization()
            .AddJsonFormatters();
           
            //Adds the authentication services to DI and configures "Bearer" as the default scheme
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {                    
                    options.Authority = identityServerUrl; //IdentityServer Address
                    options.RequireHttpsMetadata = false;
                    options.Audience = identityServerAudience;
                    if(proxyUrl != null && proxyUrl != "")
                    {
                        options.BackchannelHttpHandler = new HttpClientHandler() //Configure Proxy
                        {
                            Proxy = new WebProxy(proxyUrl)
                        };
                    }                        
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
