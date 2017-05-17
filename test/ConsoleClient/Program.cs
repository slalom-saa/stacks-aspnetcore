using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Slalom.Stacks;
using Slalom.Stacks.AspNetCore;
using Slalom.Stacks.Domain;
using Slalom.Stacks.Search;
using Slalom.Stacks.Security;
using Slalom.Stacks.Services;
using Slalom.Stacks.Text;
using EndPoint = Slalom.Stacks.Services.EndPoint;

namespace ConsoleClient
{
    [EndPoint("api/some")]
    public class SomeEndPoint : EndPoint
    {
        public override void Receive()
        {
            this.Respond(this.Request.User.Identity.Name);
        }
    }

    public class SomeController : EndPointController
    {
        [HttpGet("api/some"), ResponseCache(Duration = 15)]
        public Task Get()
        {
            return this.Send();
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