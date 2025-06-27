using JCertPreApplication.Application.Dtos.Auth;
using JCertPreApplication.Application.Features.Auth;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [Route("api/[controller]")]
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
            if (model == null)
            {
                return BadRequest("Registration model is required.");
            }

            var (succeeded, accessToken, refreshToken, user, errors) = await _authService.RegisterAsync(model);
            if (!succeeded)
            {
                return BadRequest(new { errors });
            }

            return Ok(new { accessToken, refreshToken, user });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (model == null)
            {
                return BadRequest("Login model is required.");
            }

            var (accessToken, refreshToken, user) = await _authService.LoginAsync(model.EmailorPhone, model.Password);
            if (accessToken == null || refreshToken == null || user == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            return Ok(new { accessToken, refreshToken, user });
        }

        [HttpPost("firebase-login")]
        public async Task<IActionResult> FirebaseLogin([FromBody] FirebaseLoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.FirebaseToken))
            {
                return BadRequest("Firebase token is required.");
            }

            var (accessToken, refreshToken, user) = await _authService.FirebaseLoginAsync(model.FirebaseToken);
            if (accessToken == null || refreshToken == null || user == null)
            {
                return Unauthorized("Invalid Firebase token or unable to authenticate user.");
            }

            return Ok(new { accessToken, refreshToken, user });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            var (accessToken, newRefreshToken, user) = await _authService.RefreshTokenAsync(refreshToken);
            if (accessToken == null || newRefreshToken == null || user == null)
            {
                return Unauthorized("Invalid refresh token.");
            }

            return Ok(new { accessToken, refreshToken = newRefreshToken, user });
        }
    }
}
