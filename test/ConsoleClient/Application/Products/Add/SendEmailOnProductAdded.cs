using System;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Services;

namespace ConsoleClient.Application.Products.Add
{
    public class SendEmailOnProductAdded : Service<AddProductEvent>
    {
        public override void Execute(AddProductEvent message)
        {
            Console.WriteLine("Sending mail.");

            throw new Exception("SS");
        }
    }
}