using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DnsClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SD.Api.Dtos;

namespace SD.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserServiceController : ControllerBase
    {
        private HttpClient _httpClient;
        //private readonly string _userServiceUrl = "http://localhost:5000";
        private readonly string _userServiceUrl;
        private readonly IDnsQuery _dns;
        private readonly IOptions<ServiceDiscoveryOptions> _options;
        public UserServiceController(HttpClient httpClient, IDnsQuery dns, IOptions<ServiceDiscoveryOptions> options)
        {
            _httpClient = httpClient;
            _dns = dns ?? throw new ArgumentNullException(nameof(dns));
            _options = options ?? throw new ArgumentNullException(nameof(options));

            var result = _dns.ResolveService("service.consul", _options.Value.ServiceName);
            /*
             如果服务注册用的是localhost,那么AddressList为空，则取HostName
             必须是ip地址，比如127.0.0.1
             */
            var addressList = result.First().AddressList;
            //var address = result.First().AddressList.FirstOrDefault();

            var address = addressList.Any() ?
                addressList.FirstOrDefault().ToString() :
                result.First().HostName;
            var port = result.First().Port;

            _userServiceUrl = $"http://{address}:{port}";
        }

        [HttpGet]
        public async Task<string> Get()
        {
            var service = await _dns.ResolveServiceAsync("service.consul", _options.Value.ServiceName);
            var address = service.First().AddressList.FirstOrDefault();
            var port = service.First().Port;

            var response = await _httpClient.GetAsync(_userServiceUrl + "/api/users");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }
            return string.Empty;
        }
    }
}