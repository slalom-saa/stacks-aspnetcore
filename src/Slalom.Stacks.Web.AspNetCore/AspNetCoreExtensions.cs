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
using Slalom.Stacks.Messaging.Registry;
using Slalom.Stacks.Validation;

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
                        if (string.IsNullOrWhiteSpace(content))
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
            if (result.ValidationErrors.Any(e => e.Type == ValidationType.Input))
            {
                Respond(context, result.ValidationErrors, HttpStatusCode.BadRequest);
            }
            if (result.ValidationErrors.Any(e => e.Type == ValidationType.Security))
            {
                Respond(context, result.ValidationErrors, HttpStatusCode.Unauthorized);
            }
            else if (result.ValidationErrors.Any())
            {
                Respond(context, result.ValidationErrors, HttpStatusCode.Conflict);
            }
            else if (!result.IsSuccessful)
            {
                var message = "An unhandled exception was raised on the server.  Please try again.  " + result.CorrelationId;
                Respond(context, message, HttpStatusCode.InternalServerError);
            }
            else if (result.Response != null)
            {
                Respond(context, result.Response, HttpStatusCode.OK);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NoContent;
            }
        }

        private static void Respond(HttpContext context, object content, HttpStatusCode statusCode)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            using (var inner = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(content, settings))))
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;
                context.Response.ContentLength = inner.ToArray().Length;
                inner.CopyTo(context.Response.Body);
            }
        }
    }
}