using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Newtonsoft.Json;
using Slalom.Stacks.Messaging;

namespace Slalom.Stacks.Web.AspNetCore
{
    public static class AspNetCoreExtensions
    {
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
