using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Messaging;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Slalom.Stacks.AspNetCore
{

    [Route("go")]
    public class Go : Controller
    {
        [HttpGet, AllowAnonymous]
        public string Do()
        {
            return "Asdf";
        }
    }


    internal class RootStartup
    {
        public RootStartup(IHostingEnvironment env)
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

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {


            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "ApplicationCookie",
                AutomaticAuthenticate = true,
                Events = new CookieAuthenticationEvents
                {
                    OnSigningIn = a =>
                    {
                        Console.WriteLine("OnSigningIn");
                        return Task.FromResult(0);
                    },
                    OnValidatePrincipal = a =>
                    {
                        Console.WriteLine("OnValidatePrincipal");
                        return Task.FromResult(0);
                    },
                    OnRedirectToLogin = a =>
                    {
                        Console.WriteLine("OnRedirectToLogin");
                        a.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        return Task.FromResult(0);
                    }
                }
            });

            app.UseMvc();




            //app.UseCors(b =>
            //{
            //    b.AllowAnyOrigin()
            //        .AllowAnyHeader()
            //        .AllowAnyMethod()
            //        .AllowCredentials();
            //});

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
                .AddAuthenticationSchemes("ApplicationCookie")
                .Build();

            services.AddMvc(setup =>
                    {
                        //setup.Filters.Add(new AuthorizeFilter(defaultPolicy));
                    })
                .AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); });

            Stack.Use(builder =>
            {
                builder.RegisterType<WebRequestContext>().As<IRequestContext>();
                builder.RegisterType<StacksSwaggerProvider>().AsImplementedInterfaces();
                builder.Populate(services);
            });

            return new AutofacServiceProvider(Stack.Container);
        }
    }
}