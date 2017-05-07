using System.Collections.Generic;
using Newtonsoft.Json;

namespace Slalom.Stacks.AspNetCore.Swagger.Model
{
    public abstract class SecurityScheme
    {
        public SecurityScheme()
        {
            this.Extensions = new Dictionary<string, object>();
        }

        public string Type { get; set; }

        public string Description { get; set; }

        [JsonExtensionData]
        public Dictionary<string, object> Extensions { get; private set; }
    }
}
