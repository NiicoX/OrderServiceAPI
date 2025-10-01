using Microsoft.AspNetCore.Mvc;
using OrderServiceAPI.Core.DTOs;
using OrderServiceAPI.Core.Interfaces;

namespace OrderServiceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _userService.AuthenticateAsync(request);
            if (response == null)
                return Unauthorized(new { message = "Usuario o contraseña inválidos" });

            return Ok(response);
        }
    }
}
