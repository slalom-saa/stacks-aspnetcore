﻿/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Slalom.Stacks.AspNetCore.Messaging;
using Slalom.Stacks.AspNetCore.Swagger;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Inventory;
using Slalom.Stacks.Services.Messaging;

namespace Slalom.Stacks.AspNetCore
{
    /// <summary>
    /// Extension methods for configuration AspNetCore blocks.
    /// </summary>
    public static class AspNetCoreExtensions
    {
        internal static EndPointMetaData GetEndPoint(this Stack stack, HttpRequest request)
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


            stack.Use(e =>
            {
                e.RegisterType<WebRequestContext>().As<IRequestContext>().AsSelf();
                e.RegisterType<StacksSwaggerProvider>().AsImplementedInterfaces();
                e.RegisterType<HttpDispatcher>().AsImplementedInterfaces().AsSelf().SingleInstance();
            });

            Task.Run(() => Subscribe(options));

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

        private static async Task Subscribe(AspNetCoreOptions options)
        {
            if ((options.SubscriptionUrls?.Any() ?? false) && !string.IsNullOrWhiteSpace(options.Subscriber))
            {
                var target = Startup.Stack.Container.Resolve<RemoteEndPointInventory>();
                using (var client = new HttpClient())
                {
                    while (true)
                    {
                        foreach (var url in options.SubscriptionUrls)
                        {
                            try
                            {
                                var message = new StringContent(JsonConvert.SerializeObject(new
                                {
                                    path = options.Subscriber
                                }), Encoding.UTF8, "application/json");
                                await client.PostAsync(url + "/_system/events/subscribe", message);
                            }
                            catch
                            {
                            }
                            try
                            {
                                var result = await client.GetAsync(url + "/_system/endpoints");
                                if (result.StatusCode == HttpStatusCode.OK)
                                {
                                    var content = result.Content.ReadAsStringAsync().Result;
                                    var endPoints = JsonConvert.DeserializeObject<RemoteEndPoint[]>(content);
                                    target.AddEndPoints(endPoints);
                                }
                            }
                            catch
                            {
                            }
                        }
                        await Task.Delay(5000);
                    }
                }
            }
        }
    }
}