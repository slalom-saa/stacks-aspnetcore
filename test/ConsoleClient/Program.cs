using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Slalom.Stacks;
using Slalom.Stacks.Services;
using Slalom.Stacks.AspNetCore;
using Slalom.Stacks.Domain;
using Slalom.Stacks.Search;
using Slalom.Stacks.Services.Inventory;
using Slalom.Stacks.Services.Logging;
using Slalom.Stacks.Services.Messaging;
using Slalom.Stacks.Text;
using Slalom.Stacks.Validation;
using EndPoint = Slalom.Stacks.Services.EndPoint;
using Autofac;
using Newtonsoft.Json;

namespace ConsoleClient
{
    [Route("gome")]
    public class A : Controller
    {
        [HttpGet]
        public string Do()
        {
            return "AAA";
        }
    }

    public class ProductAdded : Event
    {
        public string Name { get; }

        public ProductAdded(string name)
        {
            this.Name = name;
        }
    }


    [Request("hop")]
    public class B
    {
    }

    public class Hopped : Event
    {
    }

    [Subscribe("Hopped")]
    public class OnHopped : EndPoint<Hopped>
    {
        public override void Receive(Hopped instance)
        {
            Console.WriteLine("AAAA");
        }
    }

    [EndPoint("sales/fproducts/add", Secure = true)]
    public class AddProduct : EndPoint
    {
        public override void Receive()
        {
            this.AddRaisedEvent(new ProductAdded(DateTime.Now.ToString(CultureInfo.InvariantCulture)));

            var result = this.Send<string>(new B()).Result;

            //this.Respond(result.Response);

            this.Respond(Request.User.Identity.Name);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            using (var stack = new Stack())
            {
                stack.RunWebHost(e =>
                {
                    e.WithSubscriptions("http://localhost:5000", "http://localhost:5001");
                });
            }
        }
    }
}