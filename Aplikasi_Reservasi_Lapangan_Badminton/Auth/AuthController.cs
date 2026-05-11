using Microsoft.AspNetCore.Mvc;

namespace Aplikasi_Reservasi_Lapangan_Badminton.Auth
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            try
            {
                var roles = _authService.GetAllowedRoles();

                return Ok(new
                {
                    message = "Silakan pilih role",
                    roles = roles
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register/customer")]
        public IActionResult RegisterCustomer(RegisterRequest request)
        {
            try
            {
                request.Role = "Customer";

                var result = _authService.Register(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            try
            {
                var result = _authService.Login(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("profile/{id}")]
        public IActionResult GetProfile(int id)
        {
            try
            {
                var result = _authService.GetProfile(id);

                return Ok(new
                {
                    result.Id,
                    result.Name,
                    result.Email,
                    result.Role
                });
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logout berhasil" });
        }
    }
}