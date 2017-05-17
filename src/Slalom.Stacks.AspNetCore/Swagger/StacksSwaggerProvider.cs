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
using Slalom.Stacks.Reflection;
using Slalom.Stacks.Runtime;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Inventory;
using Slalom.Stacks.Text;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.AspNetCore.Swagger
{
    public class StacksSwaggerProvider : ISwaggerProvider
    {
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

        static readonly HashSet<Type> BuiltInScalarTypes = new HashSet<Type>
        {
            typeof(bool),
            typeof(char),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(string),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid),
            typeof(Uri)
        };

        private readonly ServiceInventory _services;
        private readonly IEnvironmentContext _environment;
        private SchemaRegistry _registry;

        public StacksSwaggerProvider(ServiceInventory services, IEnvironmentContext environment, IOptions<MvcJsonOptions> options)
        {
            _services = services;
            _environment = environment;

            _registry = new SchemaRegistry(new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

        public SwaggerDocument GetSwagger(string documentName, string host = null, string basePath = null, string[] schemes = null)
        {
            _registry.GetOrRegister(typeof(ValidationError));

            var pathItems = new Dictionary<string, PathItem>();
            foreach (var service in _services.Hosts.SelectMany(e => e.Services))
            {
                foreach (var endPoint in service.EndPoints.Where(e => e.Path != null && e.Path.StartsWith(documentName, StringComparison.OrdinalIgnoreCase)))
                {
                    if (!endPoint.Public)
                    {
                        continue;
                    }

                    //schema.Example = new SwaggerCommand { Name = "sdaf" };



                    var responses = GetResponses(endPoint, _registry);

                    var path = "/" + string.Join("/", endPoint.Path.Split('/').Skip(1));
                    var paths = endPoint.Path.Split('/');

                    var title = paths.Take(Math.Max(1, paths.Count() - 1)).Last().Replace("-", " ");
                    title = Regex.Replace(title, @"\b\w", (Match match) => match.ToString().ToUpper());


                    var pathItem = new PathItem
                    {
                        Post = GetPostOperation(endPoint, title, path, responses),
                        Get = GetGetOperation(endPoint, title, path, responses)
                    };



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
                    Title = this.GetDocumentTitle(documentName),
                    Version = "v1"
                },
                Host = host,
                BasePath = "/" + documentName,
                Paths = pathItems,
                Definitions = _registry.Definitions,
                Schemes = schemes
                //SecurityDefinitions = _settings.SecurityDefinitions
            };
            return swaggerDoc;
        }

        private Operation GetPostOperation(EndPointMetaData endPoint, string title, string path, Dictionary<string, Response> responses)
        {
            var type = endPoint.RequestType;


           


            var schema = _registry.GetOrRegister(type);

            var parameters = new List<IParameter>();
            parameters.Add(new BodyParameter { Name = "input", Schema = schema });

            var operation = new Operation
            {
                Tags = new[] { title },
                OperationId = "POST " + path,
                Consumes = new[] { "application/json" },
                Produces = new[] { "application/json" },
                Parameters = parameters,
                Responses = responses,
                Description = endPoint.Summary
            };
            return operation;
        }

        private Operation GetGetOperation(EndPointMetaData endPoint, string title, string path, Dictionary<string, Response> responses)
        {
            var type = endPoint.RequestType;

            if (!type.GetProperties().All(e => BuiltInScalarTypes.Contains(e.PropertyType)))
            {
                return null;
            }

           
            var schema = _registry.GetOrRegister(type);

            var parameters = new List<IParameter>();
            foreach (var property in endPoint.RequestProperties)
            {
                var required = type.GetProperty(property.Name).GetCustomAttribute(typeof(ValidationAttribute), true) != null;
                parameters.Add(new NonBodyParameter { Name = property.Name, Required = required, In = "query",  Description = property.Comments?.Value, Type = GetFriendlyName(property.Type) });
            }

            var operation = new Operation
            {
                Tags = new[] { title },
                OperationId = "GET " + path,
                Consumes = new[] { "application/json" },
                Produces = new[] { "application/json" },
                Parameters = parameters,
                Responses = responses,
                Description = endPoint.Summary
            };
            return operation;
        }

        private string GetDocumentTitle(string documentName)
        {
            var title = documentName.ToTitleCase() + " API";
            var environment = _environment.Resolve().EnvironmentName;
            if (!String.IsNullOrWhiteSpace(environment))
            {
                title += " - " + environment;
            }
            return title;
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