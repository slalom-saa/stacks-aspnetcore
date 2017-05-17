/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Slalom.Stacks.Services.Messaging;

namespace Slalom.Stacks.AspNetCore.Messaging
{
    public class WebRequestContext : Request
    {
        private readonly IHttpContextAccessor _accessor;

        public WebRequestContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override string GetSourceIPAddress()
        {
            return _accessor.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }


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

        protected override ClaimsPrincipal GetUser()
        {
            return _accessor.HttpContext?.User;
        }
    }
}