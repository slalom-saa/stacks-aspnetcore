using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Slalom.Stacks;
using Slalom.Stacks.AspNetCore;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Inventory;

namespace WebClient
{

    [EndPoint("test", Secure = true)]
    public class Some : EndPoint
    {
        public override void Receive()
        {
            this.Respond("Name: " + Request.User.Identity.Name + ":" + Request.User.Identity.IsAuthenticated);
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
