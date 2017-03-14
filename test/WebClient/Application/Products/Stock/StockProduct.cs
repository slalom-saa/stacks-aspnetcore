using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;

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
