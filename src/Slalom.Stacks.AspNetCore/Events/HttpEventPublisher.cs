using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Slalom.Stacks.Services.Messaging;

namespace Slalom.Stacks.AspNetCore.Events
{
    public class HttpEventPublisher : IEventPublisher
    {
        private List<string> _paths = new List<string>();
        private HttpClient _client = new HttpClient();

        public Task Publish(params EventMessage[] events)
        {
            if (events.Any() && _paths.Any())
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var content = JsonConvert.SerializeObject(new
                {
                    Content = JsonConvert.SerializeObject(events, settings)
                }, settings);
                var body = new StringContent(content, Encoding.UTF8, "application/json");

                return Task.WhenAll(_paths.Select(e => _client.PostAsync(e + "/_system/events/publish", body)));
            }
            return Task.FromResult(0);
        }

        public void Subscribe(string path)
        {
            path = path.TrimEnd('/');
            if (!_paths.Contains(path))
            {
                _paths.Add(path);
            }
        }
    }
}
