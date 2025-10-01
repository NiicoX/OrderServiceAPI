using OrderServiceAPI.Core.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderServiceAPI.Core.Interfaces
{
    public interface IUserService
    {
        Task<LoginResponse?> AuthenticateAsync(LoginRequest request);
    }
}
