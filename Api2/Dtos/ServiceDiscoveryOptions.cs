using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Dtos
{
    public class ServiceDiscoveryOptions
    {
        public string UserServiceName { get; set; }

        public ConsulOptions Consul { get; set; }
    }
}
