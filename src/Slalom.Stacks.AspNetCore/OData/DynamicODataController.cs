using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.OData;
using System.Web.OData.Extensions;
using System.Web.OData.Query;
using System.Web.OData.Routing;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Newtonsoft.Json.Linq;

namespace Slalom.Stacks.AspNetCore.OData
{
    public class DynamicODataController : ODataController
    {
    }

    public class DynamicODataController<T> : DynamicODataController
    {
        [EnableQuery]
        public IQueryable<T> Get()
        {
            var command = RootStartup.Stack.Send(this.Request.RequestUri.PathAndQuery.Split('?')[0].Trim('/')).Result;

            return command.Response as IQueryable<T>;
        }
    }
}
