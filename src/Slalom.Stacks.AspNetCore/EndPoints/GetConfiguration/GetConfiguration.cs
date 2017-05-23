/* 
 * Copyright (c) Stacks Contributors
 * 
 * This file is subject to the terms and conditions defined in
 * the LICENSE file, which is part of this source code package.
 */

using Microsoft.Extensions.Configuration;
using Slalom.Stacks.AspNetCore.Settings;
using Slalom.Stacks.Services;

namespace Slalom.Stacks.AspNetCore.EndPoints.GetConfiguration
{
    /// <summary>
    /// Gets the AspNetCore configuration.
    /// </summary>
    [EndPoint("_system/configuration/aspnetcore", Method = "GET", Public = false, Name = "Get AspNetCore Configuration", Secure = true)]
    public class GetConfiguration : EndPoint<GetConfigurationRequest, AspNetCoreOptions>
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetConfiguration" /> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public GetConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <inheritdoc />
        public override AspNetCoreOptions Receive(GetConfigurationRequest instance)
        {
            var options = new AspNetCoreOptions();
            _configuration.GetSection("Stacks:AspNetCore").Bind(options);

            return options;
        }
    }
}