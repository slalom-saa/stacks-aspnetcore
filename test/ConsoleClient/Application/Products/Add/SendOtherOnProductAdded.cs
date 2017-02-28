using System;
using Slalom.Stacks.Messaging;

namespace ConsoleClient.Application.Products.Add
{
    public class SendOtherOnProductAdded : UseCase<AddProductEvent>
    {
        public override void Execute(AddProductEvent message)
        {
            Console.WriteLine("Sending other.");
        }
    }
}