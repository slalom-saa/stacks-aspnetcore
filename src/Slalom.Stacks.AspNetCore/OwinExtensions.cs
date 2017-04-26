﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Owin.Builder;
using Microsoft.Owin.BuilderProperties;
using Owin;
using Slalom.Stacks.AspNetCore.OData;

namespace Slalom.Stacks.AspNetCore
{
    public static class OwinExtensions
    {
        //Extension method added to IApplicationBuilder
        public static IApplicationBuilder UseOwinApp(
            this IApplicationBuilder aspNetCoreApp,
            Action<IAppBuilder> configuration)
        {
            return aspNetCoreApp.UseOwin(setup => setup(next =>
            {
                AppBuilder owinAppBuilder = new AppBuilder();

                IApplicationLifetime aspNetCoreLifetime = (IApplicationLifetime)aspNetCoreApp.ApplicationServices.GetService(typeof(IApplicationLifetime));

                AppProperties owinAppProperties = new AppProperties(owinAppBuilder.Properties);

                owinAppProperties.OnAppDisposing = aspNetCoreLifetime?.ApplicationStopping ?? CancellationToken.None;

                owinAppProperties.DefaultApp = next;

                configuration(owinAppBuilder);

                return owinAppBuilder.Build<Func<IDictionary<string, object>, Task>>();
            }));
        }
    }
}
