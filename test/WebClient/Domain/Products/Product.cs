using Slalom.Stacks.Domain;

namespace WebClient.Domain.Products
{
    public class Product : AggregateRoot
    {
        public string Name { get; set; }

        public Product(string name)
        {
            this.Name = name;
        }
    }
}