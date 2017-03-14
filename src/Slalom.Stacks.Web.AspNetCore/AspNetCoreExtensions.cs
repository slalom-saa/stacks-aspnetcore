using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;

namespace Slalom.Stacks.Web.AspNetCore
{
    /// <summary>
    /// Extension methods for configuration AspNetCore blocks.
    /// </summary>
    public static class AspNetCoreExtensions
    {
        /// <summary>
        /// Starts and runs an API to access the stack.
        /// </summary>
        /// <param name="stack">The this instance.</param>
        /// <param name="configuration">The configuration routine.</param>
        public static Stack RunWebHost(this Stack stack, Action<AspNetCoreOptions> configuration = null)
        {
            var options = new AspNetCoreOptions();
            configuration?.Invoke(options);

            RootStartup.Stack = stack;
            var builder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<RootStartup>();

            if (options.Urls?.Any() ?? false)
            {
                builder.UseUrls(options.Urls);
            }

            builder.Build().Run();

            return stack;
        }

        /// <summary>
        /// Configures the application to use Stacks.
        /// </summary>
        /// <param name="app">The application to configure.</param>
        /// <param name="stack">The current stack.</param>
        /// <returns>This instance for method chaining.</returns>
        public static IApplicationBuilder UseStacks(this IApplicationBuilder app, Stack stack)
        {
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value.Trim('/');
                var registry = stack.GetServices();
                if (registry.Find(path) != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        context.Request.Body.CopyTo(stream);

                        var content = Encoding.UTF8.GetString(stream.ToArray());
                        if (String.IsNullOrWhiteSpace(content))
                        {
                            content = null;
                        }
                        var result = await stack.Send(path, content);
                        HandleResult(result, context);
                    }
                }
                else
                {
                    await next.Invoke();
                }
            });
            return app;
        }
       

        private static void HandleResult(MessageResult result, HttpContext context)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            if (result.ValidationErrors.Any(e => e.Type == Validation.ValidationType.Input))
            {
                using (var inner = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result.ValidationErrors, settings))))
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    context.Response.ContentLength = inner.ToArray().Count();
                    inner.CopyTo(context.Response.Body);
                }
            }
            if (result.ValidationErrors.Any())
            {
                using (var inner = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result.ValidationErrors, settings))))
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    context.Response.ContentLength = inner.ToArray().Count();
                    inner.CopyTo(context.Response.Body);
                }
            }
            else if (!result.IsSuccessful)
            {
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                using (var inner = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject("An unhandled exception was raised on the server.  Please try again.  " + result.CorrelationId, settings))))
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    context.Response.ContentLength = inner.ToArray().Count();
                    inner.CopyTo(context.Response.Body);
                }
            }
            else if (result.Response != null)
            {
                using (var inner = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result.Response, settings))))
                {
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int) HttpStatusCode.OK;
                    context.Response.ContentLength = inner.ToArray().Count();
                    inner.CopyTo(context.Response.Body);
                }
            }
            else
            {
                context.Response.StatusCode = (int) HttpStatusCode.NoContent;
            }
        }
    }
}