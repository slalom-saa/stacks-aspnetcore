/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Messaging;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.AspNetCore.Events.EndPoints
{
    /// <summary>
    /// Publishes events using the provided JSON text.
    /// </summary>
    [EndPoint("_system/events/publish")]
    public class PublishEvents : EndPoint<PublishEventsRequest>
    {
        private readonly IMessageGateway _messages;

        /// <summary>
        /// Initializes a new instance of the <see cref="PublishEvents" /> class.
        /// </summary>
        /// <param name="messages">The configured <see cref="IMessageGateway" />.</param>
        public PublishEvents(IMessageGateway messages)
        {
            Argument.NotNull(messages, nameof(messages));

            _messages = messages;
        }

        /// <inheritdoc />
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