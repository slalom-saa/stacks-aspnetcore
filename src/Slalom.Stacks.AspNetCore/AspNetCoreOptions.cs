/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Authentication;

namespace Slalom.Stacks.AspNetCore
{
    /// <summary>
    /// Options for AspNetCore.
    /// </summary>
    public class AspNetCoreOptions
    {
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
                    a.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    return Task.FromResult(0);
                },
                OnValidatePrincipal = async a =>
                {
                    if (a.Properties.ExpiresUtc <= DateTime.UtcNow)
                    {
                        await a.HttpContext.Authentication.SignOutAsync("Cookies");
                    }
                    else
                    {
                        await a.HttpContext.Authentication.SignInAsync("Cookies", a.Principal,
                            new AuthenticationProperties
                            {
                                ExpiresUtc = DateTime.UtcNow.AddSeconds(10)
                            });
                    }
                }
            }
        };

        internal Action<CorsPolicyBuilder> CorsOptions { get; set; } = builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        };

        internal string[] Urls { get; set; }

        /// <summary>
        /// Sets the Cookie configuration to use.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>This instance for method chaining.</returns>
        public AspNetCoreOptions WithCookieAuthentication(CookieAuthenticationOptions options)
        {
            this.CookieOptions = options;

            return this;
        }

        /// <summary>
        /// Sets the CORS configuration.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>This instance for method chaining.</returns>
        public AspNetCoreOptions WithCors(Action<CorsPolicyBuilder> options)
        {
            this.CorsOptions = options;

            return this;
        }

        /// <summary>
        /// Sets the URLs to use with local hosting.
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