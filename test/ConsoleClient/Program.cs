using System.Collections.Generic;
using System.Linq;
using System.Text;
using Slalom.Stacks;
using Slalom.Stacks.Services;
using Slalom.Stacks.AspNetCore;
using Slalom.Stacks.Domain;
using Slalom.Stacks.Search;
using Slalom.Stacks.Services.Messaging;

namespace ConsoleClient
{

    public class Product : ISearchResult
    {
        public bool Crawled { get; set; }
        public int Id { get; set; }

        public string Name { get; set; }
    }


    public class SearchProductsCommand
    {
    }

    [EndPoint("search/products")]
    public class SearchProducts : EndPoint<SearchProductsCommand, IQueryable<Product>>
    {
        public override IQueryable<Product> Receive(SearchProductsCommand instance)
        {
            return new List<Product> { new Product { Name = "Bread" }, new Product { Name = "Toast" } }.AsQueryable();
        }
    }

    [EndPoint("search/products2")]
    public class SearchProducts2 : EndPoint<SearchProductsCommand, IQueryable<Product>>
    {
        public override IQueryable<Product> Receive(SearchProductsCommand instance)
        {
            return new List<Product> { new Product { Name = "Bread" }, new Product { Name = "Toast" } }.AsQueryable();
        }
    }

    [EndPoint("products/add")]
    public class AddProduct : EndPoint<SearchProductsCommand, string>
    {
        public override string Receive(SearchProductsCommand instance)
        {
            return "Adf";
        }
    }


    public class HelloWorldRequest
    {
        public HelloWorldRequest(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }

    [EndPoint("api/hello")]
    public class HelloWorld : EndPoint<HelloWorldRequest>
    {
        public override void Receive(HelloWorldRequest instance)
        {
            //return "Hello " + instance.Name;
        }
    }

    public class GetDocumentRequest
    {
    }

    [EndPoint("document/get")]
    public class GetDocument : EndPoint<GetDocumentRequest, Document>
    {
        public override Document Receive(GetDocumentRequest instance)
        {
            return new Document("name.txt", Encoding.UTF8.GetBytes("content"));
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            using (var stack = new Stack())
            {
                stack.RunWebHost();
            }
        }
    }
}