using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Slalom.Stacks;
using Slalom.Stacks.Services;
using Slalom.Stacks.AspNetCore;
using Slalom.Stacks.Domain;
using Slalom.Stacks.Search;
using Slalom.Stacks.Services.Logging;
using Slalom.Stacks.Services.Messaging;
using Slalom.Stacks.Validation;

namespace ConsoleClient
{
    public class ProductAdded : Event
    {
        public string Name { get; }

        public ProductAdded(string name)
        {
            this.Name = name;
        }
    }

    [EndPoint("products/add")]
    public class AddProduct : EndPoint
    {
        public override void Receive()
        {
            this.AddRaisedEvent(new ProductAdded(DateTime.Now.ToString(CultureInfo.InvariantCulture)));

            this.Respond("asdfasf");    
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
                    e.WithUrls("http://localhost:5000");
                });
            }
        }
    }
}