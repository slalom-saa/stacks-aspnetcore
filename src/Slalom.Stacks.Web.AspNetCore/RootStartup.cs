using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;

namespace Slalom.Stacks.Web.AspNetCore
{
    internal class RootStartup
    {
        public static Stack Stack { get; set; }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
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