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
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Slalom.Stacks.Web.AspNetCore
{
    internal class RootStartup
    {
        public IConfigurationRoot Configuration { get; private set; }

        public static Stack Stack { get; set; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Stack.Include(this);
            //services.AddMvc();

            Stack.Use(builder =>
            {
                builder.RegisterType<StacksSwaggerProvider>().AsImplementedInterfaces();
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
                var services = Stack.GetServices().Hosts.SelectMany(e => e.Services).SelectMany(e => e.EndPoints).Select(e => e.Path).Select(e => e?.Split('/').FirstOrDefault())
                .Distinct().Where(e => e != null && !e.StartsWith("_"));
                foreach (var service in services)
                {
                    c.SwaggerEndpoint($"/swagger/{service}/swagger.json", $"{service.ToTitleCase()} API");
                }
            });

            app.Use(async (context, next) =>
            {
                if (context.Request.Method == "GET" && context.Request.Path.Value.Trim('/') == "_systems/services")
                {
                    using (var inner = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(Stack.CreatePublicRegistry(context.Request.Scheme + "://" + context.Request.Host)))))
                    {
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
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