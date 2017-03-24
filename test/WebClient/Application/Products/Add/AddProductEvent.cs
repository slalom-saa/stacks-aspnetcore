using Slalom.Stacks.Messaging;

namespace WebClient.Application.Products.Add
{
    /// <summary>
    /// Class AddProductEvent.
    /// </summary>
    public class AddProductEvent : Event
    {
        public string ProductId { get; }

        public AddProductEvent(string productId)
        {
            this.ProductId = productId;
        }
    }
}