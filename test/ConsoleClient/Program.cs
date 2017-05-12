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
using Slalom.Stacks.OData;
using Slalom.Stacks.Search;
using Slalom.Stacks.Security;
using Slalom.Stacks.Services;
using Slalom.Stacks.Text;
using Slalom.Stacks.Validation;

namespace ConsoleClient
{
    public class SearchUsers
    {
        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; }

        public SearchUsers(string text)
        {
            this.Text = text;
        }
    }

    public class User : ISearchResult
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }

    [EndPoint("go/home")]
    public class Go : EndPoint<SearchUsers, IQueryable<User>>
    {
        public override IQueryable<User> Receive(SearchUsers instance)
        {
            return new User[] { new User() }.AsQueryable();
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
                    stack.RunODataHost();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}