/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Slalom.Stacks.Services.Messaging;

namespace Slalom.Stacks.AspNetCore.Messaging
{
    /// <summary>
    /// Provides a request context for AspNetCore.
    /// </summary>
    /// <seealso cref="Slalom.Stacks.Services.Messaging.Request" />
    public class AspNetCoreRequestContext : Request
    {
        private readonly IHttpContextAccessor _accessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AspNetCoreRequestContext"/> class.
        /// </summary>
        /// <param name="accessor">The accessor.</param>
        public AspNetCoreRequestContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <inheritdoc />
        protected override string GetSession()
        {
            var context = _accessor.HttpContext;
            var key = Guid.NewGuid().ToString();
            if (context.Request.Cookies.ContainsKey("Stacks-Session"))
            {
                key = context.Request.Cookies["Stacks-Session"];
                context.Response.Cookies.Delete("Stacks-Session");
            }
            context.Response.Cookies.Append("Stacks-Session", key, new CookieOptions { Expires = DateTimeOffset.Now.AddMinutes(15) });
            return key;
        }

        /// <inheritdoc />
        protected override string GetSourceIPAddress()
        {
            var forward = _accessor.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(forward))
            {
                var target = forward.Split(',')[0];
                if (target.Contains("."))
                {
                    return target.Split(':')[0];
                }
            }
            return _accessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        /// <inheritdoc />
        protected override ClaimsPrincipal GetUser()
        {
            return _accessor.HttpContext?.User;
        }
    }
}