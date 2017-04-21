using Slalom.Stacks;
using Slalom.Stacks.Services;
using Slalom.Stacks.Web.AspNetCore;

namespace ConsoleClient
{
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