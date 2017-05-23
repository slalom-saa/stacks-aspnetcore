/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using System;
using Slalom.Stacks.Services;

namespace Slalom.Stacks.AspNetCore.EndPoints
{
    /// <summary>
    /// Gets the request IP address.  Used for troubleshooting or to help services identify the IP address that they call from.
    /// </summary>
    [EndPoint("_system/ip-address", Method = "GET", Public = false, Name = "Get IP Address")]
    public class GetIPAddress : EndPoint
    {
        /// <inheritdoc />
        public override void Receive()
        {
            this.Respond(this.Request.SourceAddress);
        }
    }
}