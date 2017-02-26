using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace Slalom.Stacks.Web.AspNetCore
{
    internal class RootStartup
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseStacks(Stack);
        }

        public static Stack Stack { get; set; }
    }
}