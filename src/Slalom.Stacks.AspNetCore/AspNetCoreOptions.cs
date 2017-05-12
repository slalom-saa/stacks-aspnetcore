/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Slalom.Stacks.Security;

namespace Slalom.Stacks.AspNetCore
{
    /// <summary>
    /// Options for AspNetCore.
    /// </summary>
    public class AspNetCoreOptions
    {
        /// <summary>
        /// Gets or sets the cookie authentication settings.
        /// </summary>
        /// <value>The cookie authentication settings.</value>
        public CookieAuthenticationSettings CookieAuthentication { get; set; } = new CookieAuthenticationSettings();

        /// <summary>
        /// Gets or sets the subscription settings.
        /// </summary>
        /// <value>The subscription settings.</value>
        public SubscriptionSettings Subscriptions { get; set; } = new SubscriptionSettings();

        internal Action<CorsPolicyBuilder> CorsOptions { get; set; } = builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                .Build();
        };

        internal string[] Urls { get; set; }

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

        internal CookieAuthenticationOptions GetCookieAuthenticationOptions()
        {
            return new CookieAuthenticationOptions
            {
                AuthenticationScheme = this.CookieAuthentication.AuthenticationScheme,
                CookieName = this.CookieAuthentication.CookieName,
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                SlidingExpiration = true,
                ExpireTimeSpan = TimeSpan.FromSeconds(1),
                DataProtectionProvider = new CookieDataProtectionProvider(this.CookieAuthentication.DataProtectionKey),
                Events = new CookieAuthenticationEvents
                {
                    OnRedirectToLogin = a =>
                    {
                        a.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                        return Task.FromResult(0);
                    },
                    OnValidatePrincipal = async a =>
                    {
                        if (a.Properties.ExpiresUtc <= DateTime.UtcNow)
                        {
                            await a.HttpContext.Authentication.SignOutAsync(this.CookieAuthentication.AuthenticationScheme);
                        }
                        else
                        {
                            a.ShouldRenew = true;
                        }
                    }
                }
            };
        }

        internal class CookieDataProtector : IDataProtector
        {
            private readonly string _salt;

            public CookieDataProtector(string salt)
            {
                _salt = salt;
            }

            public IDataProtector CreateProtector(string purpose)
            {
                return new CookieDataProtector(_salt);
            }

            public byte[] Protect(byte[] plaintext)
            {
                return Encryption.Encrypt(plaintext, _salt);
            }

            public byte[] Unprotect(byte[] protectedData)
            {
                return Encryption.Decrypt(protectedData, _salt);
            }
        }

        internal class CookieDataProtectionProvider : IDataProtectionProvider
        {
            private readonly string _key;

            public CookieDataProtectionProvider(string key)
            {
                _key = key;
            }

            public IDataProtector CreateProtector(string purpose)
            {
                return new CookieDataProtector(_key);
            }
        }

        /// <summary>
        /// Settings for cookie authentication..
        /// </summary>
        public class CookieAuthenticationSettings
        {
            /// <summary>
            /// Gets or sets the authentication scheme name.
            /// </summary>
            /// <value>The authentication scheme name.</value>
            public string AuthenticationScheme { get; set; } = "Cookies";

            /// <summary>
            /// Gets or sets the name of the cookie.
            /// </summary>
            /// <value>The name of the cookie.</value>
            public string CookieName { get; set; } = ".AspNetCore.Cookies";

            /// <summary>
            /// Gets or sets the data protection key that is used to encrypt the cookie.
            /// </summary>
            /// <value>The data protection key that is used to encrypt the cookie.</value>
            public string DataProtectionKey { get; set; } = @"Stacks";

            /// <summary>
            /// Gets or sets the expire time span.
            /// </summary>
            /// <value>The expire time span.</value>
            public TimeSpan ExpireTimeSpan { get; set; } = TimeSpan.FromMinutes(15);
        }

        /// <summary>
        /// Settings for subscriptions.
        /// </summary>
        public class SubscriptionSettings
        {
            /// <summary>
            /// Gets or sets the local URL of the service that will be called on publish.
            /// </summary>
            /// <value>The local URL of the service that will be called on publish.</value>
            public string Local { get; set; }

            /// <summary>
            /// Gets or sets the remote URLs to subscribe to.
            /// </summary>
            /// <value>The remote URLs to subscribe to.</value>
            public string[] Remote { get; set; } = new string[0];
        }
    }
}