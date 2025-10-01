using OrderServiceAPI.Core.DTOs;
using OrderServiceAPI.Core.Interfaces;
using OrderServiceAPI.Infrastructure.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderServiceAPI.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly JwtTokenGenerator _tokenGenerator;

        public UserService(JwtTokenGenerator tokenGenerator)
        {
            _tokenGenerator = tokenGenerator;
        }

        public Task<LoginResponse?> AuthenticateAsync(LoginRequest request)
        {
            // ⚠️ Por ahora validación hardcodeada (demo)
            if (request.Username == "admin" && request.Password == "1234")
            {
                var token = _tokenGenerator.GenerateToken(request.Username, "Admin");
                return Task.FromResult<LoginResponse?>(new LoginResponse
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddMinutes(60)
                });
            }

            return Task.FromResult<LoginResponse?>(null);
        }
    }
}
