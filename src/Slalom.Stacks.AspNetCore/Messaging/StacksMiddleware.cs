/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */


// TODO: Fix setting header issue
// TODO: Swagger from relative URLs

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Slalom.Stacks.Services.Messaging;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.AspNetCore.Messaging
{
    public class StacksMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly Stack _stack;

        public StacksMiddleware(RequestDelegate next, Stack stack)
        {
            _next = next;
            _stack = stack;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == "OPTIONS")
            {
                await _next(context);
                return;
            }

            var endPoint = _stack.GetEndPoint(context.Request);
            if (endPoint != null)
            {
                try
                {
                    if (context.Request.Method == "GET")
                    {
                        if (context.Request.Query.Any())
                        {
                            var content = new JObject();
                            foreach (var item in context.Request.Query)
                            {
                                content.Add(item.Key, item.Value.ToString());
                            }
                            var result = await _stack.Send(endPoint.Path, content.ToString());
                            HandleResult(result, context);
                        }
                        else
                        {
                            var result = await _stack.Send(endPoint.Path, null);
                            HandleResult(result, context);
                        }
                    }
                    else if (context.Request.Method == "POST")
                    {
                        using (var stream = new MemoryStream())
                        {
                            context.Request.Body.CopyTo(stream);

                            var content = Encoding.UTF8.GetString(stream.ToArray());
                            if (string.IsNullOrWhiteSpace(content))
                            {
                                content = null;
                            }
                            var result = await _stack.Send(endPoint.Path, content);
                            HandleResult(result, context);
                        }
                    }
                    else
                    {
                        await _next(context);
                    }
                }
                catch (Exception exception)
                {
                    _stack.Logger.Error(exception, "Failed to execute endpoint in the Stacks middleware.");
                    throw;
                }
            }
            else
            {
                await _next(context);
            }
        }


        private static void HandleResult(MessageResult result, HttpContext context)
        {
            if (result.ValidationErrors.Any(e => e.Type == ValidationType.Input))
            {
                Respond(context, result.ValidationErrors, HttpStatusCode.BadRequest);
            }
            else if (result.ValidationErrors.Any(e => e.Type == ValidationType.Security))
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
                if (result.Response is Document)
                {
                    Respond(context, (Document) result.Response, HttpStatusCode.OK);
                }
                else
                {
                    Respond(context, result.Response, HttpStatusCode.OK);
                }
            }
            else
            {
                context.Response.StatusCode = (int) HttpStatusCode.NoContent;
            }
        }

        private static void Respond(HttpContext context, Document content, HttpStatusCode statusCode)
        {
            using (var stream = new MemoryStream(content.Content))
            {
                context.Response.ContentType = MimeTypes.GetMimeType(Path.GetExtension(content.Name));
                context.Response.StatusCode = (int) statusCode;
                context.Response.ContentLength = content.Content.Length;
                stream.CopyTo(context.Response.Body);
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
                context.Response.StatusCode = (int) statusCode;
                context.Response.ContentLength = inner.ToArray().Length;
                inner.CopyTo(context.Response.Body);
            }
        }
    }
}