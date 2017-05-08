using System;
using Newtonsoft.Json.Serialization;
using Slalom.Stacks.AspNetCore.Swagger.Model;

namespace Slalom.Stacks.AspNetCore.Swagger.Generator
{
    public interface ISchemaFilter
    {
        void Apply(Schema model, SchemaFilterContext context);
    }

    public class SchemaFilterContext
    {
        public SchemaFilterContext(
            Type systemType,
            JsonContract jsonContract,
            ISchemaRegistry schemaRegistry)
        {
            this.SystemType = systemType;
            this.JsonContract = jsonContract;
            this.SchemaRegistry = schemaRegistry;
        }

        public Type SystemType { get; private set; }

        public JsonContract JsonContract { get; private set; }

        public ISchemaRegistry SchemaRegistry { get; private set; }
    }
}