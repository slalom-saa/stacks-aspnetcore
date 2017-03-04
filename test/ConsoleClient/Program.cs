using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Slalom.Stacks;
using Slalom.Stacks.Logging;
using Slalom.Stacks.Web.AspNetCore;

namespace ConsoleClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var stack = new Stack())
            {
                stack.UseSimpleConsoleLogging();

                stack.RunWebHost();
            }
        }
    }
}
