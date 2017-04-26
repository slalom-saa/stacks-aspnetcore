using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using Slalom.Stacks.Search;

namespace Slalom.Stacks.AspNetCore
{
    public class Product: ISearchResult
    {
        public bool Crawled { get; set; }

        public string Name { get; set; }

        public int Id { get; set; }
    }

    public class ProductsController : ODataController
    {
        [EnableQuery]
        public IQueryable<Product> Get()
        {
            return new List<Product>() { new Product(), new Product() }.AsQueryable();
        }
    }
}