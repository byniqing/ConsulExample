using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Identity.Services
{
    public interface IUserService
    {
        /// <summary>
        /// 检查手机号是否已经注册
        /// 如果没有注册就注册一个用户
        /// </summary>
        /// <param name="phone"></param>
        Task<int> CheckOrCreate(string phone);
    }
}
