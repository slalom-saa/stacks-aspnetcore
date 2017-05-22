using System;
using Microsoft.AspNetCore.Builder;

namespace Slalom.Stacks.AspNetCore.Swagger.Application
{
    public static class SwaggerBuilderExtensions
    {
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<SwaggerMiddleware>();
        }
    }
}