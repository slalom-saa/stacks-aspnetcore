using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Slalom.Stacks.Services;

namespace Slalom.Stacks.AspNetCore.Events.EndPoints
{
    [EndPoint("_system/events/subscribe")]
    public class Subscribe : EndPoint<SubscribeRequest>
    {
        private readonly HttpEventPublisher _publisher;

        public Subscribe(HttpEventPublisher publisher)
        {
            _publisher = publisher;
        }

        public override void Receive(SubscribeRequest instance)
        {
            _publisher.Subscribe(instance.Path);
        }
    }
}
