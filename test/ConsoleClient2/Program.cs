using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Slalom.Stacks;
using Slalom.Stacks.AspNetCore;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Logging;
using Slalom.Stacks.Text;

namespace ConsoleClient2
{
    public class ProductAdded
    {
        public string Name { get; }

        public ProductAdded(string name)
        {
            this.Name = name;
        }
    }

    [Subscribe("ProductAdded")]
    public class OnProductAdded : EndPoint<ProductAdded>
    {
        public override void Receive(ProductAdded instance)
        {
            instance.Name.OutputToJson();
        }
    }

    public class Hopped : Event
    {
    }


    [EndPoint("hop")]
    public class Hop : EndPoint
    {
        public override void Receive()
        {
            Console.WriteLine(Request.User.Identity.Name);

            this.Respond("Hello hopped");

            this.AddRaisedEvent(new Hopped());
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
                    e.WithUrls("http://localhost:5001");
                });
            }
        }
    }

}
