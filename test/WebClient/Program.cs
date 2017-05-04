using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Slalom.Stacks;
using Slalom.Stacks.AspNetCore;
using Slalom.Stacks.Services;

namespace WebClient
{

    [EndPoint("test")]
    public class Some : EndPoint
    {
        public override void Receive()
        {
            base.Respond("123456");
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
