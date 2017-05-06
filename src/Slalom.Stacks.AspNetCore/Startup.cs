using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Slalom.Stacks.AspNetCore.EndPoints;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Messaging;
using Slalom.Stacks.Text;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Slalom.Stacks.AspNetCore
{
    internal class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public static Stack Stack { get; set; }
        public static AspNetCoreOptions Options { get; set; }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseCors(Options.CorsOptions);

            app.UseCookieAuthentication(Options.CookieOptions);

            app.UseMvc();
            

            var services = Stack.GetServices();
            if (services.EndPoints.Any(e => e.Public && !String.IsNullOrWhiteSpace(e.Path) && !e.Path.StartsWith("_")))
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    var current = services
                                       .Hosts.SelectMany(e => e.Services)
                                       .Where(e => e.EndPoints.Any(x => x.Public))
                                       .SelectMany(e => e.EndPoints)
                                       .Select(e => e.Path)
                                       .Select(e => e?.Split('/').FirstOrDefault())
                                       .Distinct()
                                       .Where(e => e != null && !e.StartsWith("_"))
                                       .OrderBy(e => e);
                    foreach (var service in current)
                    {
                        c.SwaggerEndpoint($"/swagger/{service}/swagger.json", $"{service.ToTitleCase()} API");
                    }
                });
            }

            app.UseStacks(Stack);
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Stack.Include(this);

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            var defaultPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes("Cookies")
                .Build();

            var mvc = services.AddMvc(setup =>
                {
                    setup.Filters.Add(new AuthorizeFilter(defaultPolicy));
                })
                .AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); });

            foreach (var assembly in Stack.Assemblies)
            {
                mvc.AddApplicationPart(assembly);
            }

            Stack.Use(builder =>
            {
                builder.Populate(services);
            });
            return new AutofacServiceProvider(Stack.Container);
        }
    }
}