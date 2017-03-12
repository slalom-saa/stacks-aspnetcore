using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;
using Swashbuckle.AspNetCore.Swagger;

namespace Slalom.Stacks.Web.AspNetCore
{
    public class ApiProvider : IApiDescriptionGroupCollectionProvider
    {
        public ApiDescriptionGroupCollection ApiDescriptionGroups => this.Get();

        public ApiDescriptionGroupCollection Get()
        {
            var desc = new ApiDescription();
            desc.ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor();
            //desc.GroupName = "Fred";
            desc.HttpMethod = "GET";
            desc.ActionDescriptor = new ControllerActionDescriptor
            {
                ActionName = "GetME"
            };

            var items = new List<ApiDescription> { desc };

            var list = new List<ApiDescriptionGroup> { new ApiDescriptionGroup("group", items) };

            var group = new ApiDescriptionGroupCollection(list, 1);

            return group;
        }
    }


    internal class RootStartup
    {
        public IConfigurationRoot Configuration { get; private set; }

        public static Stack Stack { get; set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Stack.Use(builder =>
            {
                builder.RegisterType<ApiProvider>().AsImplementedInterfaces();
            });

            //services.AddMvc();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });

            Stack.Use(builder =>
            {
                builder.Populate(services);
            });

            return new AutofacServiceProvider(Stack.Container);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Method == "GET" && context.Request.Path.Value.Trim('/') == "_services")
                {
                    using (var inner = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Stack.CreatePublicRegistry(context.Request.Scheme + "://" + context.Request.Host)))))
                    {
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int) HttpStatusCode.OK;
                        context.Response.ContentLength = inner.ToArray().Count();
                        inner.CopyTo(context.Response.Body);
                    }
                }
                else
                {
                    await next.Invoke();
                }
            });

            app.UseStacks(Stack);
        }
    }
}