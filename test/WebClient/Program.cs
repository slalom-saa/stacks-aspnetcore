using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Slalom.Stacks;
using Slalom.Stacks.AspNetCore;

namespace WebClient
{
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
