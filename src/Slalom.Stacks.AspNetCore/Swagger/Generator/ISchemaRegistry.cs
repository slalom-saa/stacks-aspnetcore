using System;
using System.Collections.Generic;
using Slalom.Stacks.AspNetCore.Swagger.Model;

namespace Slalom.Stacks.AspNetCore.Swagger.Generator
{
    public interface ISchemaRegistry
    {
        Schema GetOrRegister(Type type);

        IDictionary<string, Schema> Definitions { get; }
    }
}
