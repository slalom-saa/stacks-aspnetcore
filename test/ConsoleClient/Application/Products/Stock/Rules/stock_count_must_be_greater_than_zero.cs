using System.Collections.Generic;
using Slalom.Stacks.Messaging.Validation;
using Slalom.Stacks.Validation;

namespace ConsoleClient.Application.Products.Stock.Rules
{
    public class stock_count_must_be_greater_than_zero : BusinessRule<StockProductCommand>
    {
        public override IEnumerable<ValidationError> Validate(StockProductCommand instance)
        {
            if (instance.ItemCount < 5)
            {
                yield return "The stock count must be in multiples of 5.";
            }
        }
    }
}
