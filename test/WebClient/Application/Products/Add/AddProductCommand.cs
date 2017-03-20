using Slalom.Stacks.Messaging;
using Slalom.Stacks.Validation;

namespace WebClient.Application.Products.Add
{
    /// <summary>
    /// Adds a product to the something.
    /// </summary>
    public class AddProductCommand 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddProductCommand" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="count">The count.</param>
        public AddProductCommand(string name, int count)
        {
            this.Name = name;
            this.Count = count;
        }

        /// <summary>
        /// Gets the count.
        /// </summary>
        /// <value>The count.</value>
        public int Count { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        [NotNull("Name is required.")]
        public string Name { get; }
    }
}