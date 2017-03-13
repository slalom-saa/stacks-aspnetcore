using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using ConsoleClient.Application.Products.Add;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Slalom.Stacks;
using Slalom.Stacks.Logging;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Services.Registry;
using Slalom.Stacks.Text;
using Slalom.Stacks.Web.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ConsoleClient
{
    public class StacksProvider
    {
        private readonly ServiceRegistry _services;

        public StacksProvider(ServiceRegistry services)
        {
            _services = services;
        }

        public SwaggerDocument GetSwagger(string documentName)
        {
            var host = _services.Hosts.First();

            var operation = new Operation
            {
                Tags = new[] { "Yello" },
                OperationId = "GET ME",
                Consumes = new[] { "application/json" },
                Produces = new[] { "application/json" },
                Parameters = null, // parameters can be null but not empty
                Responses = new Dictionary<string, Response>()
            };

            var pathItem = new PathItem
            {
                Get = operation
            };

            var di = new Dictionary<string, PathItem>();
            di.Add("get", pathItem);

            var x = new Dictionary<string, Schema>();
            x.Add("o", new Schema
            {
                });

            var swaggerDoc = new SwaggerDocument
            {
                Info = new Info
                {
                    Title = "Hello"
                },
                Host = "asdf",
                BasePath = "asdfasdf",
                Paths = di,
                Definitions = x
                //SecurityDefinitions = _settings.SecurityDefinitions
            };
            return swaggerDoc;
        }
    }


    public class Program
    {
        public static void Main(string[] args)
        {
            using (var stack = new Stack())
            {

                var p = new StacksSwaggerProvider(stack.GetServices());

                p.GetSwagger("Document").OutputToJson();

                stack.RunWebHost();
            }









        }
    }
}
