/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using System.Linq;
using Microsoft.AspNetCore.Http;
using Slalom.Stacks.Services;
using Slalom.Stacks.Services.Inventory;

namespace Slalom.Stacks.AspNetCore.EndPoints
{
    /// <summary>
    /// Gets the endpoints that are exposed by HTTP.  This is mainly used by the system for peer-to-peer discovery but can be scanned to see what endpoints are available.
    /// </summary>
    [EndPoint("_system/endpoints", Method = "GET", Name = "Get Available Endpoints", Public = false)]
    public class GetEndPoints : EndPoint<GetEndPointsRequest, RemoteService>
    {
        private readonly IHttpContextAccessor _context;
        private readonly ServiceInventory _services;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetEndPoints" /> class.
        /// </summary>
        /// <param name="services">The current service inventory.</param>
        /// <param name="context">The context.</param>
        public GetEndPoints(ServiceInventory services, IHttpContextAccessor context)
        {
            _services = services;
            _context = context;
        }

        /// <inheritdoc />
        public override RemoteService Receive(GetEndPointsRequest instance)
        {
            var url = this.GetBaseUrl();
            var endPoints = _services.EndPoints.Where(e => e.Public && !string.IsNullOrWhiteSpace(e.Path) && !e.Path.StartsWith("_"));

            return new RemoteService
            {
                Path = _context.HttpContext.Request.Scheme + "://" + _context.HttpContext.Request.Host,
                EndPoints = endPoints.Select(e => new RemoteEndPoint(e.Path, url + "/" + e.Path, e.Method)).ToList()
            };
        }

        private string GetBaseUrl()
        {
            var request = _context.HttpContext.Request;

            var host = request.Host.ToUriComponent();

            var pathBase = request.PathBase.ToUriComponent();

            return $"{request.Scheme}://{host}{pathBase}";
        }
    }
}