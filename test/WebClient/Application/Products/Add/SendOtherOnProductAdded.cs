using System;
using Slalom.Stacks.Services;

namespace WebClient.Application.Products.Add
{
    public class SendOtherOnProductAdded : EventUseCase<AddProductEvent>
    {
        public override void Execute(AddProductEvent message)
        {
            Console.WriteLine("Sending other.");
        }
    }
}