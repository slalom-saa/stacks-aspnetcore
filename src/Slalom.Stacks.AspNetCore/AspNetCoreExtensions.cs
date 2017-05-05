using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Slalom.Stacks.AspNetCore.EndPoints;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Inventory;
using Slalom.Stacks.Text;
using Slalom.Stacks.Validation;

namespace Slalom.Stacks.AspNetCore
{
    /// <summary>
    /// Extension methods for configuration AspNetCore blocks.
    /// </summary>
    public static class AspNetCoreExtensions
    {
        public static EndPointMetaData GetEndPoint(this Stack stack, HttpRequest request)
        {
            var path = request.Path.Value.Trim('/');
            var inventory = stack.GetServices();
            return inventory.Find(path);
        }

        /// <summary>
        /// Starts and runs an API to access the stack.
        /// </summary>
        /// <param name="stack">The this instance.</param>
        /// <param name="configuration">The configuration routine.</param>
        public static Stack RunWebHost(this Stack stack, Action<AspNetCoreOptions> configuration = null)
        {
            var options = new AspNetCoreOptions();
            configuration?.Invoke(options);

            Startup.Stack = stack;
            Startup.Options = options;
            var builder = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>();

            if (options.Urls?.Any() ?? false)
            {
                builder.UseUrls(options.Urls);
            }

            if ((options.SubscriptionUrls?.Any() ?? false) && !String.IsNullOrWhiteSpace(options.Subscriber))
            {
                using (var client = new HttpClient())
                {
                    foreach (var url in options.SubscriptionUrls)
                    {
                        var content = new StringContent(JsonConvert.SerializeObject(new
                        {
                            path = options.Subscriber
                        }), Encoding.UTF8, "application/json");
                        client.PostAsync(url + "/_system/events/subscribe", content).Wait();
                    }
                }
            }

            builder.Build().Run();

            return stack;
        }

        /// <summary>
        /// Configures the application to use Stacks.
        /// </summary>
        /// <param name="app">The application to configure.</param>
        /// <param name="stack">The current stack.</param>
        /// <returns>This instance for method chaining.</returns>
        public static IApplicationBuilder UseStacks(this IApplicationBuilder app, Stack stack)
        {
            return app.UseMiddleware<StacksMiddleware>(stack);
        }
    }
}