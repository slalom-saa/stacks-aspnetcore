using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.OData.Extensions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Owin;
using Slalom.Stacks.AspNetCore.OData;
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Messaging;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Slalom.Stacks.AspNetCore
{
    internal class RootStartup
    {
        public IConfigurationRoot Configuration { get; private set; }

        public static Stack Stack { get; set; }

        public RootStartup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

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

            app.UseOwinApp(owinApp =>
            {
                HttpConfiguration configuration = new HttpConfiguration();
                
                var server = new HttpServer(configuration);
                configuration.Routes.MapDynamicODataServiceRoute(
                    "odata",
                    "",
                    server);
                configuration.AddODataQueryFilter();
               // var y = configuration.Services.GetService(typeof(IHttpControllerSelector));
                configuration.Services.Replace(typeof(IHttpControllerSelector), new ODataControllerSelector(configuration));
                owinApp.UseWebApi(configuration);

            });

            app.UseStacks(Stack);
        }
    }
}