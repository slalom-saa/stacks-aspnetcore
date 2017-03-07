using System;
using Slalom.Stacks.Messaging;
using Slalom.Stacks.Services;

namespace ConsoleClient.Application.Products.Add
{
    public class SendEmailOnProductAdded : EndPoint<AddProductEvent>
    {
        public override void Receive(AddProductEvent message)
        {
            Console.WriteLine("Sending mail.");

            throw new Exception("SS");
        }
    }
}