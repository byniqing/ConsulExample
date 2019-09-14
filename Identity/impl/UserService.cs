using Identity.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.impl
{
    public class UserService : IUserService
    {
        public async Task<int> CheckOrCreate(string phone)
        {
            return 1;
            //throw new NotImplementedException();
        }
    }
}
