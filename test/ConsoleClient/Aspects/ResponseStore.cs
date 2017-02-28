using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Slalom.Stacks.Messaging.Logging;

namespace ConsoleClient.Aspects
{
    public class ResponseStore : IResponseStore
    {
        public Task Append(ResponseEntry entry)
        {
            Console.WriteLine(JsonConvert.SerializeObject(entry, Formatting.Indented));

            return Task.FromResult(0);
        }
    }
}