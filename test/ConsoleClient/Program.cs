using System;
using System.Linq;
using Slalom.Stacks;
using Slalom.Stacks.Web.AspNetCore;

namespace ConsoleClient
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