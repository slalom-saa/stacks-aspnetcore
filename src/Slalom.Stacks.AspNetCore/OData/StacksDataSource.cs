using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.OData;
using System.Web.OData.Query;
using Microsoft.OData.Core.UriParser.Semantic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Library;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Slalom.Stacks.AspNetCore.OData
{
    public class StacksDataSource : IDataSource
    {
        public string Name { get; }
        public EdmModel Model { get; }
        public EdmEntityObjectCollection Get(ODataQueryOptions queryOptions)
        {
            var edmType = queryOptions.Context.Path.GetEdmType() as IEdmCollectionType;
            var entityType = (edmType as IEdmCollectionType).ElementType.AsEntity();
            var table = (entityType.Definition as EdmEntityType).Name;

            List<ExpandedNavigationSelectItem> expands = new List<ExpandedNavigationSelectItem>();
            if (queryOptions.SelectExpand != null)
            {
                foreach (var item in queryOptions.SelectExpand.SelectExpandClause.SelectedItems)
                {
                    var expande = item as ExpandedNavigationSelectItem;
                    if (expande == null)
                        continue;
                    expands.Add(expande);
                }
            }

            EdmEntityObjectCollection collection = new EdmEntityObjectCollection(new EdmCollectionTypeReference(edmType));

            return Get(edmType, queryOptions, expands);
        }

        EdmEntityObjectCollection Get(IEdmCollectionType edmType, ODataQueryOptions sqlCmd, List<ExpandedNavigationSelectItem> expands = null)
        {
            var entityType = edmType.ElementType.AsEntity();

            var target = sqlCmd.ApplyTo((IQueryable)new List<Product> {new Product(), new Product()}.AsQueryable());

            EdmEntityObjectCollection collection = new EdmEntityObjectCollection(new EdmCollectionTypeReference(edmType));


            foreach (var item in target)
            {
                var ser = JObject.FromObject(item);
                var entity = new EdmEntityObject(entityType);
                foreach (var property in ser.Properties())
                {
                    entity.TrySetPropertyValue(property.Name, property.ToObject(typeof(Product).GetProperty(property.Name).PropertyType));
                }

                collection.Add(entity);

            }

            return collection;
        }

        public int GetCount(ODataQueryOptions queryOptions)
        {
            throw new NotImplementedException();
        }

        public EdmEntityObject Get(string key, ODataQueryOptions queryOptions)
        {
            throw new NotImplementedException();
        }

        public string Create(IEdmEntityObject entity)
        {
            throw new NotImplementedException();
        }

        public int Delete(string key, IEdmType elementType)
        {
            throw new NotImplementedException();
        }

        public int Merge(string key, IEdmEntityObject entity)
        {
            throw new NotImplementedException();
        }

        public int Replace(string key, IEdmEntityObject entity)
        {
            throw new NotImplementedException();
        }

        public IEdmObject InvokeFunction(IEdmFunction action, JObject parameterValues, ODataQueryOptions queryOptions = null)
        {
            throw new NotImplementedException();
        }

        public int GetFuncResultCount(IEdmFunction func, JObject parameterValues, ODataQueryOptions queryOptions)
        {
            throw new NotImplementedException();
        }

        public IEdmObject DoAction(IEdmAction action, JObject parameterValues)
        {
            throw new NotImplementedException();
        }

        public Action<RequestInfo> BeforeExcute { get; set; }
        public Action<RequestInfo> AfrerExcute { get; set; }
    }
}
