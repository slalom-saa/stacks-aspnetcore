using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Slalom.Stacks.AspNetCore.Swagger.Generator;
using Slalom.Stacks.AspNetCore.Swagger.Model;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Inventory;
using Slalom.Stacks.Text;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.AspNetCore.Swagger
{
    public class StacksSwaggerProvider : ISwaggerProvider
    {
        private readonly ServiceInventory _services;

        public StacksSwaggerProvider(ServiceInventory services, IOptions<MvcJsonOptions> options)
        {
            _services = services;
        }

        private static string GetFriendlyName(string name)
        {
            var type = Type.GetType(name, false);

            if (type == typeof(int))
                return "int";
            else if (type == typeof(short))
                return "short";
            else if (type == typeof(byte))
                return "byte";
            else if (type == typeof(bool))
                return "bool";
            else if (type == typeof(long))
                return "long";
            else if (type == typeof(float))
                return "float";
            else if (type == typeof(double))
                return "double";
            else if (type == typeof(decimal))
                return "decimal";
            else if (type == typeof(string))
                return "string";

            return name;
        }

        public SwaggerDocument GetSwagger(string documentName, string host = null, string basePath = null, string[] schemes = null)
        {
            var registry = new SchemaRegistry(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            registry.GetOrRegister(typeof(ValidationError));

            var pathItems = new Dictionary<string, PathItem>();
            foreach (var service in _services.Hosts.SelectMany(e => e.Services))
            {
                foreach (var endPoint in service.EndPoints.Where(e => e.Path != null && e.Path.StartsWith(documentName, StringComparison.OrdinalIgnoreCase)))
                {
                    if (!endPoint.Public)
                    {
                        continue;
                    }
                    var type = endPoint.RequestType;
                    var schema = registry.GetOrRegister(type);

                    //schema.Example = new SwaggerCommand { Name = "sdaf" };

                    var parameters = new List<IParameter>();
                    parameters.Add(new BodyParameter { Name = "input", Schema = schema });
                    foreach (var property in endPoint.RequestProperties)
                    {
                        //parameters.Add(new NonBodyParameter { Name = property.Name, Description = property.Comments?.Value, Type = GetFriendlyName(property.Type) });
                    }

                    var responses = GetResponses(endPoint, registry);

                    
    
                    var paths = endPoint.Path.Split('/');
                    var title = paths.Take(Math.Max(1, paths.Count() - 1)).Last().Replace("-", " ");
                    title = Regex.Replace(title, @"\b\w", (Match match) => match.ToString().ToUpper());
                    var operation = new Operation
                    {
                        Tags = new[] { title },
                        OperationId = endPoint.RequestType.Name.Split(',')[0].Split('.').Last().Replace("Request", "").ToDelimited("-"),
                        Consumes = new[] { "application/json" },
                        Produces = new[] { "application/json" },
                        Parameters = parameters,
                        Responses = responses,
                        Description = endPoint.Summary
                    };

                    var pathItem = new PathItem
                    {
                        Post = operation,
                        //Get = operation
                    };

                    var path = "/" + string.Join("/", endPoint.Path.Split('/').Skip(1));

                    if (!pathItems.ContainsKey(path))
                    {
                        pathItems.Add(path, pathItem);
                    }
                }
            }

            var swaggerDoc = new SwaggerDocument
            {
                Info = new Info
                {
                    Title = documentName.ToTitleCase() + " API"
                },
                Host = host,
                BasePath = "/" + documentName,
                Paths = pathItems,
                Definitions = registry.Definitions,
                Schemes = schemes
                //SecurityDefinitions = _settings.SecurityDefinitions
            };
            return swaggerDoc;
        }

        private static Dictionary<string, Response> GetResponses(EndPointMetaData endPoint, SchemaRegistry registry)
        {
            var responses = new Dictionary<string, Response>();
            if (endPoint.ResponseType == null)
            {
                responses.Add("204", new Response
                {
                    Description = "No content is returned from this endpoint.  A 204 status code is returned when execution completes successfully."
                });
            }
            else
            {
                var responseType = endPoint.ResponseType;
                var responseSchema = registry.GetOrRegister(responseType);
                responses.Add("200", new Response
                {
                    Description = responseType.GetComments()?.Summary,
                    Schema = responseSchema
                });
            }
            var builder = new StringBuilder();
            foreach (var property in endPoint.RequestProperties.Where(e => e.Validation != null))
            {
                builder.AppendLine(property.Validation + "    ");
            }
            foreach (var source in endPoint.Rules.Where(e => e.RuleType == ValidationType.Input))
            {
                builder.AppendLine(source.Name.ToTitleCase() + ".    ");
            }
            if (builder.Length > 0)
            {
                responses.Add("400", new Response
                {
                    Schema = registry.GetOrRegister(typeof(ValidationError)),
                    Description = builder.ToString()
                });
            }
            builder.Clear();
            foreach (var source in endPoint.Rules.Where(e => e.RuleType == ValidationType.Business))
            {
                builder.AppendLine(source.Name.ToTitleCase() + ".    ");
            }
            if (builder.Length > 0)
            {
                responses.Add("409", new Response
                {
                    Schema = registry.GetOrRegister(typeof(ValidationError)),
                    Description = builder.ToString()
                });
            }
            builder.Clear();
            foreach (var source in endPoint.Rules.Where(e => e.RuleType == ValidationType.Security))
            {
                builder.AppendLine(source.Name.ToTitleCase() + ".    ");
            }
            if (builder.Length > 0)
            {
                responses.Add("403", new Response
                {
                    Schema = registry.GetOrRegister(typeof(ValidationError)),
                    Description = builder.ToString()
                });
            }
            return responses;
        }
    }
}