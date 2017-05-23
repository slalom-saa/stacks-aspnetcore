using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Slalom.Stacks;
using Slalom.Stacks.AspNetCore;
using Slalom.Stacks.OData;
using Slalom.Stacks.Search;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Inventory;

namespace WebClient
{
    public class User : ISearchResult
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class SeachUsersRequest
    {
    }

    [EndPoint("search/users")]
    public class SearchUsers : EndPoint<SeachUsersRequest, IQueryable<User>>
    {
        public override IQueryable<User> Receive(SeachUsersRequest instance)
        {
            return this.Search.Search<User>();
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            using (var stack = new Stack())
            {
                stack.RunODataHost();
            }
        }
    }
}
