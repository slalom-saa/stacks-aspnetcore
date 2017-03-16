using System;
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
    public class a_product_should_have_last_name : BusinessRule<AddProductCommand>
    {
        public override IEnumerable<ValidationError> Validate(AddProductCommand instance)
        {
            yield break;
        }
    }

    public class a_product_should_have_first_name : BusinessRule<AddProductCommand>
    {
        public override IEnumerable<ValidationError> Validate(AddProductCommand instance)
        {
            yield break;
        }
    }

    public class user_must_be_registered : SecurityRule<AddProductCommand>
    {
        public override IEnumerable<ValidationError> Validate(AddProductCommand instance)
        {
            Console.WriteLine(this.User?.Identity.Name);
            yield break;
        }
    }

    /// <summary>
    /// Adds a product.  Yay.
    /// </summary>
    [EndPoint("catalog/products/add")]
    public class AddProduct : UseCase<AddProductCommand, AddProductEvent>
    {
        public override async Task<AddProductEvent> ExecuteAsync(AddProductCommand command)
        {
            var target = new Product("name");

            await this.Domain.Add(target);

            var stock = await this.Send("asdf", new StockProductCommand(command.Count));
            if (!stock.IsSuccessful)
            {
                await this.Domain.Remove(target);

                throw new ChainFailedException(this.Request, stock);
            }

            return new AddProductEvent(target.Id);
        }
    }

    /// <summary>
    /// Adds a product.  Yay.  Version 2.
    /// </summary>
    [EndPoint("catalog/products/add", Version = 2)]
    public class AddProduct_v2 : UseCase<AddProductCommand, AddProductEvent>
    {
        public override async Task<AddProductEvent> ExecuteAsync(AddProductCommand command)
        {
            var target = new Product("name");

            await this.Domain.Add(target);

            var stock = await this.Send("sadfas", new StockProductCommand(command.Count));
            if (!stock.IsSuccessful)
            {
                await this.Domain.Remove(target);
            }

            return new AddProductEvent(target.Id);
        }
    }
}   