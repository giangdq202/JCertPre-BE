using JCertPreApplication.Application.Dtos.Auth;
using JCertPreApplication.Application.Features.Auth;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            // Model validation is handled by middleware if using [Required] attributes
            // Or we can throw ApiException here if needed
            
            var (accessToken, refreshToken, user) = await _authService.RegisterAsync(model);
            return Ok(new { accessToken, refreshToken, user });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var (accessToken, refreshToken, user) = await _authService.LoginAsync(model.Email, model.Password);
            return Ok(new { accessToken, refreshToken, user });
        }

        [HttpPost("firebase-login")]
        public async Task<IActionResult> FirebaseLogin([FromBody] FirebaseLoginModel model)
        {
            var (accessToken, refreshToken, user) = await _authService.FirebaseLoginAsync(model.FirebaseToken);
            return Ok(new { accessToken, refreshToken, user });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var (accessToken, newRefreshToken, user) = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(new { accessToken, refreshToken = newRefreshToken, user });
        }
    }
}
