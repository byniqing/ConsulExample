using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SD.Api.Dtos
{
    public class ConsulOptions
    {
        public string HttpEndpoint { get; set; }

        public DnsEndpoint DnsEndpoint { get; set; }
    }
}
