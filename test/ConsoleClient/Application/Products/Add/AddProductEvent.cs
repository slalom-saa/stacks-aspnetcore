using Slalom.Stacks.Messaging;

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