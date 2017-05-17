using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Slalom.Stacks;
using Slalom.Stacks.AspNetCore;
using Slalom.Stacks.Domain;
using Slalom.Stacks.Search;
using Slalom.Stacks.Security;
using Slalom.Stacks.Services;
using Slalom.Stacks.Text;
using Slalom.Stacks.Validation;

namespace ConsoleClient
{
    public class InnerRequest
    {
        /// <summary>
        /// Gets or sets the outer property.
        /// </summary>
        /// <value>The outer property.</value>
        [NotNull("outer")]
        public string OuterProperty { get; set; }
    }

   
    public class OneRequest
    {
        public string InnerProperty { get; set; }

        public InnerRequest Outer { get; set; }

    }

    [EndPoint("sales/promo-codes/other")]
    public class Other : EndPoint<InnerRequest>
    {
    }

    [EndPoint("sales/promo-codes/go")]
    public class RequestEndPoint : EndPoint<OneRequest>
    {
        public override void Receive(OneRequest instance)
        {
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