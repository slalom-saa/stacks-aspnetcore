using System;
using System.Collections.Generic;
using Slalom.Stacks.AspNetCore.Swagger.Model;

namespace Slalom.Stacks.AspNetCore.Swagger.Generator
{
    public class SchemaRegistrySettings
    {
        public SchemaRegistrySettings()
        {
            this.CustomTypeMappings = new Dictionary<Type, Func<Schema>>();
            this.SchemaIdSelector = (type) => type.FriendlyId(false);
            this.SchemaFilters = new List<ISchemaFilter>();
        }

        public IDictionary<Type, Func<Schema>> CustomTypeMappings { get; private set; }

        public bool DescribeAllEnumsAsStrings { get; set; }

        public bool DescribeStringEnumsInCamelCase { get; set; }

        public Func<Type, string> SchemaIdSelector { get; set; }

        public bool IgnoreObsoleteProperties { get; set; }

        public IList<ISchemaFilter> SchemaFilters { get; private set; }

        internal SchemaRegistrySettings Clone()
        {
            return new SchemaRegistrySettings
            {
                CustomTypeMappings = this.CustomTypeMappings,
                DescribeAllEnumsAsStrings = this.DescribeAllEnumsAsStrings,
                DescribeStringEnumsInCamelCase = this.DescribeStringEnumsInCamelCase,
                IgnoreObsoleteProperties = this.IgnoreObsoleteProperties,
                SchemaIdSelector = this.SchemaIdSelector,
                SchemaFilters = this.SchemaFilters
            };
        }
    }
}