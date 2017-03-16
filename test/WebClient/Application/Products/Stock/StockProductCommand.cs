using Slalom.Stacks.Messaging;

namespace WebClient.Application.Products.Stock
{
    public class StockProductCommand : Command
    {
        public int ItemCount { get; }

        public StockProductCommand(int itemCount) 
        {
            this.ItemCount = itemCount;
        }
    }
}