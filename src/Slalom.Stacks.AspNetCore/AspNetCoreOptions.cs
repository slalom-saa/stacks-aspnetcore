namespace Slalom.Stacks.AspNetCore
{
    /// <summary>
    /// Options for AspNetCore blocks.
    /// </summary>
    public class AspNetCoreOptions
    {
        internal string[] Urls { get; set; }

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