using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Slalom.Stacks.AspNetCore.EndPoints;
using Slalom.Stacks.Services.Inventory;
using Slalom.Stacks.Services.Messaging;

namespace Slalom.Stacks.AspNetCore
{
    public class HttpDispatcher : IRemoteMessageDispatcher
    {
        private readonly IHttpContextAccessor _context;
        private readonly RemoteEndPointInventory _endPoints;

        public HttpDispatcher(IHttpContextAccessor context, RemoteEndPointInventory endPoints)
        {
            _context = context;
            _endPoints = endPoints;
        }

        public bool CanDispatch(Request request)
        {
            return Enumerable.Any<RemoteEndPoint>(_endPoints.EndPoints, e => e.Path == request.Path);
        }

        public async Task<MessageResult> Dispatch(Request request, ExecutionContext parentContext, TimeSpan? timeout = null)
        {
            var context = new ExecutionContext(request, parentContext);
            var cookieContainer = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler
            {
                UseCookies = true,
                UseDefaultCredentials = true,
                CookieContainer = cookieContainer
            };

            if (_context?.HttpContext?.Request?.Cookies != null)
            {
                foreach (var cookie in _context.HttpContext.Request.Cookies)
                {
                    cookieContainer.Add(new Uri(_context.HttpContext.Request.Scheme + "://" + _context.HttpContext.Request.Host), new Cookie(cookie.Key, cookie.Value));
                }
            }

            var endPoint = Enumerable.First<RemoteEndPoint>(_endPoints.EndPoints, e => e.Path == request.Path);
            using (var client = new HttpClient(handler))
            {
                var result = await client.GetAsync(endPoint.FullPath);
                var content = await result.Content.ReadAsStringAsync();

                context.Response = content;

                return new MessageResult(context);
            }
        }

        
    }
}