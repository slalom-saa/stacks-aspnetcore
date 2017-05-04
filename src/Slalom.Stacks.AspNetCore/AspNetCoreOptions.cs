namespace Slalom.Stacks.AspNetCore
{
    /// <summary>
    /// Options for AspNetCore blocks.
    /// </summary>
    public class AspNetCoreOptions
    {
        internal string Subscriber { get; set; }

        internal string[] SubscriptionUrls { get; set; }

        internal string[] Urls { get; set; }

        /// <summary>
        /// Creates subscriptions at the specified URLs.
        /// </summary>
        /// <param name="subscriber">The subsciber URL to call, not including path.</param>
        /// <param name="urls">The urls to subscribe to.</param>
        /// <returns>This instance for method chaining.</returns>
        public AspNetCoreOptions WithSubscriptions(string subscriber, params string[] urls)
        {
            this.Subscriber = subscriber;
            this.SubscriptionUrls = urls;

            return this;
        }

        /// <summary>
        /// Sets the URLs to use with hosting.
        /// </summary>
        /// <param name="urls">The urls to use.</param>
        /// <returns>This instance for method chaining.</returns>
        public AspNetCoreOptions WithUrls(params string[] urls)
        {
            this.Urls = urls;

            return this;
        }
    }
}