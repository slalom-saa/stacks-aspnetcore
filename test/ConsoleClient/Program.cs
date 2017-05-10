using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Slalom.Stacks;
using Slalom.Stacks.AspNetCore;
using Slalom.Stacks.Security;
using Slalom.Stacks.Services;
using Slalom.Stacks.Text;

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
    public class AddItem : EndPoint<AddRequest, string>
    {
        public override string Receive(AddRequest instance)
        {
            Console.WriteLine(instance.Name);

            return this.Request.User.Identity.Name;
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                using (var stack = new Stack())
                {
                    stack.RunWebHost();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}