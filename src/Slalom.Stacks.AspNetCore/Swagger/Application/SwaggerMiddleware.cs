using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Slalom.Stacks.Serialization;
using Slalom.Stacks.Services.Messaging;
using Slalom.Stacks.Services.OpenApi;

namespace Slalom.Stacks.AspNetCore.Swagger.Application
{
    public class SwaggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IMessageGateway _messages;
        private readonly TemplateMatcher _requestMatcher;

        public SwaggerMiddleware(
            RequestDelegate next,
            IMessageGateway messages,
            IOptions<MvcJsonOptions> mvcJsonOptions)
        {
            _next = next;
            _messages = messages;
            _requestMatcher = new TemplateMatcher(TemplateParser.Parse("swagger/{documentName}/swagger.json"), new RouteValueDictionary());
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string documentName;
            if (!RequestingSwaggerDocument(httpContext.Request, out documentName))
            {
                await _next(httpContext);
                return;
            }

            var basePath = string.IsNullOrEmpty(httpContext.Request.PathBase)
                ? "/"
                : httpContext.Request.PathBase.ToString();

            var request = _messages.Send("_system/open-api").Result;
            if (!request.IsSuccessful)
            {
                throw new System.Exception("adf");
            }

            var document = request.Response as OpenApiDocument;

            RespondWithSwaggerJson(httpContext.Response, document);
        }

        private bool RequestingSwaggerDocument(HttpRequest request, out string documentName)
        {
            documentName = null;
            if (request.Method != "GET") return false;

            var routeValues = new RouteValueDictionary();
            if (!_requestMatcher.TryMatch(request.Path, routeValues) || !routeValues.ContainsKey("documentName")) return false;

            documentName = routeValues["documentName"].ToString();
            return true;
        }

        private void RespondWithSwaggerJson(HttpResponse response, OpenApiDocument swagger)
        {
            using (var inner = new MemoryStream(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(swagger, DefaultSerializationSettings.Instance))))
            {
                response.ContentType = "application/json";
                response.StatusCode = 200;
                response.ContentLength = inner.ToArray().Length;
                inner.CopyTo(response.Body);
            }
        }
    }
}
