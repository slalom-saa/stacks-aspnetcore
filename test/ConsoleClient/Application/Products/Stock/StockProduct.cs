using System.Collections.Generic;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;
using Slalom.Stacks.Validation;

namespace ConsoleClient.Application.Products.Stock
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
