using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UsersController : ControllerBase
    {
        private IConfiguration _configuration;
        ILogger<UsersController> _logger;
        public UsersController(IConfiguration configuration,ILogger<UsersController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        [HttpGet]
        public void Get()
        {
            _logger.LogDebug("ces");
            var ip = HttpContext.Connection.LocalIpAddress.ToString();
            Response.WriteAsync($"the ip is :{ip},from UsersController");
        }
    }
}