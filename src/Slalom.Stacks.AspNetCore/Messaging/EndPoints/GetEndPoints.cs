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

namespace Slalom.Stacks.AspNetCore.Messaging.EndPoints
{
    /// <summary>
    /// Gets the endpoints that this service exposes.
    /// </summary>
    [EndPoint("_system/endpoints")]
    public class GetEndPoints : EndPoint
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
        public override void Receive()
        {
            var url = this.GetBaseUrl();
            var endPoints = _services.EndPoints.Where(e => e.Public && !string.IsNullOrWhiteSpace(e.Path) && !e.Path.StartsWith("_"));

            this.Respond(new RemoteService
            {
                Path = _context.HttpContext.Request.Scheme + "://" + _context.HttpContext.Request.Host,
                EndPoints = endPoints.Select(e => new RemoteEndPoint(e.Path, url + "/" + e.Path)).ToList()
            });
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