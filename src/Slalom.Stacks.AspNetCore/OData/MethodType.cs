using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Slalom.Stacks.AspNetCore.OData
{
    public enum MethodType
    {
        Replace,
        Merge,
        Delete,
        Create,
        Get,
        Count,
        Function,
        Action
    }
}
