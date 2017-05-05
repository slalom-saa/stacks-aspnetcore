using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.DataProtection;

namespace Slalom.Stacks.AspNetCore
{
    /// <summary>
    /// Options for AspNetCore blocks.
    /// </summary>
    public class AspNetCoreOptions
    {
        internal string Subscriber { get; set; }

        internal string[] SubscriptionUrls { get; set; } = new string[0];

        internal string[] Urls { get; set; }

        internal Action<CorsPolicyBuilder> CorsOptions { get; set; } = builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        };

        internal CookieAuthenticationOptions CookieOptions { get; set; } = new CookieAuthenticationOptions
        {
            AuthenticationScheme = "Cookies",
            CookieName = ".AspNetCore.Cookies",
            AutomaticAuthenticate = true,
            AutomaticChallenge = true,
            SlidingExpiration = true,
            ExpireTimeSpan = TimeSpan.FromSeconds(1),
            DataProtectionProvider = DataProtectionProvider.Create(new DirectoryInfo(@"c:\shared-auth-ticket-keys\")),
            Events = new CookieAuthenticationEvents
            {
                OnRedirectToLogin = a =>
                {
                    a.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                    return Task.FromResult(0);
                }
            }
        };

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

        public AspNetCoreOptions WithCors(Action<CorsPolicyBuilder> options)
        {
            this.CorsOptions = options;

            return this;
        }

        public AspNetCoreOptions WithCookieAuthentication(CookieAuthenticationOptions options)
        {
            this.CookieOptions = options;

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