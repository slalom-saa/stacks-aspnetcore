using System.Collections.Generic;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Services;
using Slalom.Stacks.Validation;

namespace ConsoleClient.Application.Products.Stock
{
    public class StockProduct : EndPoint<StockProductCommand>
    {
        public override void Receive(StockProductCommand message)
        {
            //throw new Exception("XX");
        }
    }
}
