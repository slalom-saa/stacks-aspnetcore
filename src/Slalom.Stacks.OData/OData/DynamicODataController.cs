using System.Linq;
using System.Web.OData;

namespace Slalom.Stacks.OData.OData
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
