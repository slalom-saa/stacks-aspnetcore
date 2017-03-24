using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Messaging.Registry;
using Slalom.Stacks.Reflection;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Slalom.Stacks.Web.AspNetCore
{
    [Route("some")]
    public class Go
    {
        [HttpGet]
        public string Get()
        {
            return "asdfa";
        }
    }   

    internal class RootStartup
    {
        public IConfigurationRoot Configuration { get; private set; }

        public static Stack Stack { get; set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Stack.Include(this);

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddMvc()
                .AddJsonOptions(options =>
                {
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
                });

            Stack.Use(builder =>
            {
                builder.RegisterType<WebRequestContext>().As<IRequestContext>();
                builder.RegisterType<StacksSwaggerProvider>().AsImplementedInterfaces();
                builder.Populate(services);

            });

            return new AutofacServiceProvider(Stack.Container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                var services = Stack.GetServices().Hosts.SelectMany(e => e.Services).Where(e => e.EndPoints.Any(x => x.Public)).SelectMany(e => e.EndPoints).Select(e => e.Path).Select(e => e?.Split('/').FirstOrDefault())
                .Distinct().Where(e => e != null && !e.StartsWith("_")).OrderBy(e => e);
                foreach (var service in services)
                {

                    c.SwaggerEndpoint($"/swagger/{service}/swagger.json", $"{service.ToTitleCase()} API");
                }
            });

            app.UseStacks(Stack);
        }
    }
}