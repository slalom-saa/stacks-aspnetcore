using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Slalom.Stacks.Messaging;

namespace Slalom.Stacks.Web.AspNetCore
{
    public class WebRequestContext : Request
    {
        private readonly IHttpContextAccessor _accessor;

        public WebRequestContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        protected override ClaimsPrincipal GetUser()
        {
            return _accessor.HttpContext?.User;
        }
    }
}