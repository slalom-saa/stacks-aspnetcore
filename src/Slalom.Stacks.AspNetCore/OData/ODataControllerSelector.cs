using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Slalom.Stacks.AspNetCore.OData;
using Slalom.Stacks.Services;

namespace Slalom.Stacks.AspNetCore
{
    public class ODataControllerSelector : DefaultHttpControllerSelector
    {
        private readonly HttpConfiguration _configuration;
      
        public ODataControllerSelector(HttpConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var expected =  base.SelectController(request);

            var service = RootStartup.Stack.GetServices().Find(request.RequestUri.PathAndQuery.Split('?')[0].Trim('/'));
            var entityType = service.ResponseType.GetGenericArguments()[0];
            expected.ControllerType = typeof(DynamicODataController<>).MakeGenericType(entityType);

            return expected;
        }
    }
}