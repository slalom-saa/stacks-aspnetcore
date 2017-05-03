using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Slalom.Stacks;
using Slalom.Stacks.AspNetCore;
using Slalom.Stacks.Services;
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

    class Program
    {
        static void Main(string[] args)
        {
            using (var stack = new Stack())
            {
                Thread.Sleep(3000);
                //stack.UseAkka();

                //stack.Schedule(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2), new ConsumeEventFeedRequest("http://localhost:5000"));

                stack.RunWebHost(e =>
                {
                    e.WithUrls("http://localhost:5001")
                     .WithSubscriptions("http://localhost:5000");
                });
            }
        }
    }

}
