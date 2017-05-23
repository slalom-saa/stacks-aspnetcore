using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.OData.Extensions;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Owin;
using Slalom.Stacks.AspNetCore;
using Slalom.Stacks.AspNetCore.Messaging;
using Slalom.Stacks.AspNetCore.Swagger;
using Slalom.Stacks.AspNetCore.Swagger.UI.Application;
using Slalom.Stacks.OData.OData;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Messaging;

namespace Slalom.Stacks.OData
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
                builder.RegisterType<AspNetCoreRequestContext>().As<IRequestContext>();
                builder.Populate(services);
            });

            return new AutofacServiceProvider(Stack.Container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseMvc();
            app.UseSwaggerUI();

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