using System;
using System.IO;
using Autofac;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Slalom.Stacks.Messaging;

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
        public static Stack RunHost(this Stack stack, Action<AspNetCoreOptions> configuration = null)
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
                var registry = stack.Container.Resolve<LocalRegistry>();
                if (registry.Find(path) != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        context.Request.Body.CopyTo(stream);

                        var content = Encoding.UTF8.GetString(stream.ToArray());

                        var result = await stack.Send(path, content);
                        if (result.ValidationErrors.Any())
                        {
                            using (var inner = new MemoryStream(Encoding.UTF32.GetBytes(JsonConvert.SerializeObject(result.ValidationErrors))))
                            {
                                context.Response.ContentType = "application/json";
                                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                context.Response.ContentLength = inner.ToArray().Count();
                                inner.CopyTo(context.Response.Body);
                            }
                        }
                        else if (!result.IsSuccessful)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            using (var inner = new MemoryStream(Encoding.UTF32.GetBytes(JsonConvert.SerializeObject("An unhandled exception was raised on the server.  Please try again.  " + result.CorrelationId))))
                            {
                                context.Response.ContentType = "application/json";
                                context.Response.StatusCode = (int)HttpStatusCode.OK;
                                context.Response.ContentLength = inner.ToArray().Count();
                                inner.CopyTo(context.Response.Body);
                            }
                        }
                        else if (result.Response != null)
                        {
                            using (var inner = new MemoryStream(Encoding.UTF32.GetBytes(JsonConvert.SerializeObject(result.Response))))
                            {
                                context.Response.ContentType = "application/json";
                                context.Response.StatusCode = (int)HttpStatusCode.OK;
                                context.Response.ContentLength = inner.ToArray().Count();
                                inner.CopyTo(context.Response.Body);
                            }
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                        }
                    }
                }
                else
                {
                    await next.Invoke();
                }
            });
            return app;
        }

        /// <summary>
        /// Configures the application to use Stacks.
        /// </summary>
        /// <param name="app">The application to configure.</param>
        /// <param name="configuration">The configuration routine.</param>
        /// <returns>This instance for method chaining.</returns>
        public static IApplicationBuilder UseStacks(this IApplicationBuilder app, Action<Stack> configuration = null)
        {
            var stack = new Stack();
            app.Use(async (context, next) =>
            {
                var path = context.Request.Path.Value.Trim('/');
                var registry = stack.Container.Resolve<LocalRegistry>();
                if (registry.Find(path) != null)
                {
                    using (var stream = new MemoryStream())
                    {
                        context.Request.Body.CopyTo(stream);

                        var content = Encoding.UTF8.GetString(stream.ToArray());

                        var result = await stack.Send(path, content);
                        if (result.ValidationErrors.Any())
                        {
                            using (var inner = new MemoryStream(Encoding.UTF32.GetBytes(JsonConvert.SerializeObject(result.ValidationErrors))))
                            {
                                context.Response.ContentType = "application/json";
                                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                context.Response.ContentLength = inner.ToArray().Count();
                                inner.CopyTo(context.Response.Body);
                            }
                        }
                        else if (!result.IsSuccessful)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            using (var inner = new MemoryStream(Encoding.UTF32.GetBytes(JsonConvert.SerializeObject("An unhandled exception was raised on the server.  Please try again.  " + result.CorrelationId))))
                            {
                                context.Response.ContentType = "application/json";
                                context.Response.StatusCode = (int)HttpStatusCode.OK;
                                context.Response.ContentLength = inner.ToArray().Count();
                                inner.CopyTo(context.Response.Body);
                            }
                        }
                        else if (result.Response != null)
                        {
                            using (var inner = new MemoryStream(Encoding.UTF32.GetBytes(JsonConvert.SerializeObject(result.Response))))
                            {
                                context.Response.ContentType = "application/json";
                                context.Response.StatusCode = (int)HttpStatusCode.OK;
                                context.Response.ContentLength = inner.ToArray().Count();
                                inner.CopyTo(context.Response.Body);
                            }
                        }
                        else
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
                        }
                    }
                }
                else
                {
                    await next.Invoke();
                }
            });
            configuration?.Invoke(stack);
            return app;
        }
    }
}