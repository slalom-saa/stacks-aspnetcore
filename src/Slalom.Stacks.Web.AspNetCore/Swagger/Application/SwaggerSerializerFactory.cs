using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Swashbuckle.AspNetCore.Swagger
{
    public class SwaggerSerializerFactory
    {
        internal static JsonSerializer Create(IOptions<MvcJsonOptions> applicationJsonOptions)
        {
            return new JsonSerializer
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = applicationJsonOptions.Value.SerializerSettings.Formatting,
                ContractResolver = new SwaggerContractResolver(applicationJsonOptions.Value.SerializerSettings)
            };
        }
    }
}
