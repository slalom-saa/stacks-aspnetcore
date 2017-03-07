using System.Collections.Generic;
using System.Threading.Tasks;
using ConsoleClient.Application.Products.Stock;
using ConsoleClient.Domain.Products;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Messaging.Exceptions;
using Slalom.Stacks.Messaging.Validation;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Registry;
using Slalom.Stacks.Validation;

namespace ConsoleClient.Application.Products.Add
{
    /// <summary>
    /// Adds a product.  Yay.
    /// </summary>
    [EndPoint("products/add")]
    public class AddProduct : EndPoint<AddProductCommand, AddProductEvent>
    {
        public override async Task<AddProductEvent> ReceiveAsync(AddProductCommand command)
        {
            var target = new Product("name");

            await this.Domain.Add(target);

            var stock = await this.Send(new StockProductCommand(command.Count));
            if (!stock.IsSuccessful)
            {
                await this.Domain.Remove(target);

                throw new ChainFailedException(this.Request.Message, stock);
            }

            return new AddProductEvent(target.Id);
        }
    }

    /// <summary>
    /// Adds a product.  Yay.  Version 2.
    /// </summary>
    [EndPoint("products/add", Version = 2)]
    public class AddProduct_v2 : EndPoint<AddProductCommand, AddProductEvent>
    {
        public override async Task<AddProductEvent> ReceiveAsync(AddProductCommand command)
        {
            var target = new Product("name");

            await this.Domain.Add(target);

            var stock = await this.Send(new StockProductCommand(command.Count));
            if (!stock.IsSuccessful)
            {
                await this.Domain.Remove(target);

                throw new ChainFailedException(this.Request.Message, stock);
            }

            return new AddProductEvent(target.Id);
        }
    }
}