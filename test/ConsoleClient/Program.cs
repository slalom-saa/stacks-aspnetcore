using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Slalom.Stacks;
using Slalom.Stacks.AspNetCore;
using Slalom.Stacks.Services;

namespace ConsoleClient
{
    public class AddRequest
    {
        public AddRequest(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }

    [EndPoint("add-item")]
    public class AddItem : EndPoint<AddRequest>
    {
        public override void Receive(AddRequest instance)
        {
            Console.WriteLine(instance.Name);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var root = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();

            var sub = root.GetSection("Stacks:Subscriptions").Get<SubscriptionOptions>();

            using (var stack = new Stack())
            {
                stack.RunWebHost();
            }
        }
    }
}