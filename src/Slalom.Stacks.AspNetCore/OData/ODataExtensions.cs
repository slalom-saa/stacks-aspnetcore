using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData.Batch;
using System.Web.OData.Builder;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using Microsoft.AspNetCore.Routing;
using Microsoft.OData.Edm;

namespace Slalom.Stacks.AspNetCore.OData
{
    public static class ODataExtensions
    {
        public static ODataRoute MapDynamicODataServiceRoute(
            this HttpRouteCollection routes,
            string routeName,
            string routePrefix,
            HttpServer httpServer)
        {
            IList<IODataRoutingConvention> routingConventions = ODataRoutingConventions.CreateDefault();
            routingConventions.Insert(0, new DynamicODataRoutingConvention());
            return MapDynamicODataServiceRoute(
                routes,
                routeName,
                routePrefix,
                GetModelFuncFromRequest(),
                new DefaultODataPathHandler(),
                routingConventions,
                batchHandler: new DynamicODataBatchHandler(httpServer));
        }

        private static ODataRoute MapDynamicODataServiceRoute(
            HttpRouteCollection routes,
            string routeName,
            string routePrefix,
            Func<HttpRequestMessage, IEdmModel> modelProvider,
            IODataPathHandler pathHandler,
            IEnumerable<IODataRoutingConvention> routingConventions,
            ODataBatchHandler batchHandler)
        {
            if (!string.IsNullOrEmpty(routePrefix))
            {
                int prefixLastIndex = routePrefix.Length - 1;
                if (routePrefix[prefixLastIndex] == '/')
                {
                    routePrefix = routePrefix.Substring(0, routePrefix.Length - 1);
                }
            }
            if (batchHandler != null)
            {
                batchHandler.ODataRouteName = routeName;
                string batchTemplate = string.IsNullOrEmpty(routePrefix)
                    ? ODataRouteConstants.Batch
                    : routePrefix + '/' + ODataRouteConstants.Batch;
                routes.MapHttpBatchRoute(routeName + "Batch", batchTemplate, batchHandler);
            }
            DynamicODataPathRouteConstraint routeConstraint = new DynamicODataPathRouteConstraint(
                pathHandler,
                modelProvider,
                routeName,
                routingConventions);
            DynamicODataRoute odataRoute = new DynamicODataRoute(routePrefix, routeConstraint);
            routes.Add(routeName, odataRoute);


            //routes.Add("sdf", new ODataRoute("adf", new AllConstraint(pathHandler,
            //    modelProvider,
            //    routeName,
            //    routingConventions)));

            return odataRoute;
        }

        internal static IEdmType GetEdmType(this ODataPath path)
        {
            return path.Segments[0].GetEdmType(path.EdmType);

        }

        internal static Func<HttpRequestMessage, IEdmModel> GetModelFuncFromRequest()
        {
            return request =>
            {
                string odataPath = request.Properties[Constants.CustomODataPath] as string ?? string.Empty;
                string[] segments = odataPath.Split('/');
                string dataSource = segments[0];
                request.Properties[Constants.ODataDataSource] = dataSource;

                ODataModelBuilder odataMetadataBuilder = new ODataConventionModelBuilder();
                odataMetadataBuilder.Namespace = odataMetadataBuilder.ContainerName = "WebApplication1";

                var products = odataMetadataBuilder.EntitySet<Product>("Products");

                IEdmModel model = odataMetadataBuilder.GetEdmModel();
                request.Properties[Constants.CustomODataPath] = string.Join("/", segments, 1, segments.Length - 1);
                return model;
            };
        }
    }
}
