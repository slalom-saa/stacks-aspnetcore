using Slalom.Stacks.Messaging;

namespace WebClient.Application.Products.Stock
{
    [EndPoint("shipping/products/stock")]
    public class StockProduct : UseCase<StockProductCommand>
    {
        public override void Execute(StockProductCommand message)
        {
            //throw new Exception("XX");
        }
    }
}