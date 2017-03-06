using System.Collections.Generic;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Services;
using Slalom.Stacks.Validation;

namespace ConsoleClient.Application.Products.Stock
{
    public class StockProduct : Service<StockProductCommand>
    {
        public override void Execute(StockProductCommand message)
        {
            //throw new Exception("XX");
        }
    }
}
