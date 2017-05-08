using System;
using Microsoft.AspNetCore.Builder;

namespace Slalom.Stacks.AspNetCore.Swagger.Application
{
    public static class SwaggerBuilderExtensions
    {
        public static IApplicationBuilder UseSwagger(
            this IApplicationBuilder app,
            Action<SwaggerOptions> setupAction = null)
        {
            var options = new SwaggerOptions();
            setupAction?.Invoke(options);

            return app.UseMiddleware<SwaggerMiddleware>(options);
        }
    }
}