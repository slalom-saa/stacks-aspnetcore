using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.XPath;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Services.Registry;
using Slalom.Stacks.Text;
using Swashbuckle.AspNetCore.Swagger;

namespace Swashbuckle.AspNetCore.SwaggerGen
{
    public class StacksSwaggerProvider : ISwaggerProvider
    {
        private readonly ServiceRegistry _services;

        public StacksSwaggerProvider(ServiceRegistry services)
        {
            _services = services;
        }

        public SwaggerDocument GetSwagger(string documentName, string host = null, string basePath = null, string[] schemes = null)
        {
            var registry = new SchemaRegistry(new Newtonsoft.Json.JsonSerializerSettings());
            var schemas = new Dictionary<string, Schema>();

            var pathItems = new Dictionary<string, PathItem>();
            foreach (var service in _services.CreatePublicRegistry(host).Hosts.SelectMany(e => e.Services))
            {
                foreach (var endPoint in service.EndPoints.Where(e => e.Path.StartsWith(documentName, StringComparison.OrdinalIgnoreCase)))
                {
                    var type = Type.GetType(endPoint.RequestType);
                    var schema = registry.GetOrRegister(type);
                    if (!schemas.ContainsKey(type.Name))
                    {
                        schemas.Add(type.Name, schema);
                    }
                    //schema.Example = new SwaggerCommand { Name = "sdaf" };

                    var parameters = new List<IParameter>();
                    parameters.Add(new BodyParameter { Name = "input", Schema = schema });

                    var paths = endPoint.Path.Split('/');
                    var operation = new Operation
                    {
                        Tags = new[] { paths.Take(Math.Max(1, paths.Count() - 1)).Last().ToTitleCase() },
                        OperationId = endPoint.RequestType.Split(',')[0].Split('.').Last().Replace("Command", "").ToDelimited("-"),
                        Consumes = new[] { "application/json" },
                        Produces = new[] { "application/json" },
                        Parameters = parameters,
                        Responses = new Dictionary<string, Response>(),
                        Description = endPoint.Summary
                    };

                    var pathItem = new PathItem
                    {
                        Post = operation
                    };

                    var path = "/" + String.Join("/", endPoint.Path.Split('/').Skip(1));

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
                Definitions = schemas,
                Schemes = schemes
                //SecurityDefinitions = _settings.SecurityDefinitions
            };
            return swaggerDoc;
        }
    }
}