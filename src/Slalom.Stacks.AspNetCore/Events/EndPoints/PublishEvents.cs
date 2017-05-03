using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Messaging;

namespace Slalom.Stacks.AspNetCore.Events.EndPoints
{
    [EndPoint("_system/events/publish")]
    public class PublishEvents : EndPoint<PublishEventsRequest>
    {
        private readonly IMessageGateway _messages;

        public PublishEvents(IMessageGateway messages)
        {
            _messages = messages;
        }

        public override void Receive(PublishEventsRequest instance)
        {
            var content = JsonConvert.DeserializeObject<JObject[]>(instance.Content);
            foreach (var item in content)
            {
                var name = item["name"].Value<string>();
                _messages.Publish(name, item.ToString());
            }
        }
    }
}