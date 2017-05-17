using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Slalom.Stacks.Services;

namespace Slalom.Stacks.AspNetCore.EndPoints
{
    [EndPoint("_system/ip-address")]
    public class GetIPAddress : EndPoint
    {
        public override void Receive()
        {
            this.Respond(this.Request.SourceAddress);
        }
    }
}
