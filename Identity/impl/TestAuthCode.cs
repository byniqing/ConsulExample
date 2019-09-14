using Identity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.impl
{
    public class TestAuthCode : IAuthCodeService
    {
        public bool Validate(string phone, string authCode)
        {
            return true;
        }
    }
}
