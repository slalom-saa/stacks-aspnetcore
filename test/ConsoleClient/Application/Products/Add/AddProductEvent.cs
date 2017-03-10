using Slalom.Stacks.Messaging;
using Slalom.Stacks.Messaging.Events;

namespace ConsoleClient.Application.Products.Add
{
    public class AddProductEvent : Event
    {
        public string ProductId { get; }

        public AddProductEvent(string productId)
        {
            this.ProductId = productId;
        }
    }
}