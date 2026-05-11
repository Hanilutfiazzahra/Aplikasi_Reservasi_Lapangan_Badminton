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

        // 1. Menampilkan pilihan role
        // GET /api/auth/roles
        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            try
            {
                var roles = _authService.GetAllowedRoles();

                return Ok(new
                {
                    message = "Silakan pilih role",
                    pilihan = new[]
                    {
                "1. Admin",
                "2. Customer"
            },
                    roles = roles
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 2. Register khusus customer
        // POST /api/auth/register/customer
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

        // 3. Login admin atau customer
        // POST /api/auth/login
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

        // 4. Melihat profile berdasarkan id
        // GET /api/auth/profile/{id}
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

        // 5. Logout sederhana
        // POST /api/auth/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logout berhasil" });
        }

        // 6. Menu admin setelah login berhasil
        // GET /api/auth/admin-menu
        [HttpGet("admin-menu")]
        public IActionResult GetAdminMenu()
        {
            return Ok(new
            {
                message = "Login admin berhasil. Silakan pilih menu admin.",
                menu = new[]
                {
                    "1. Lihat data reservasi",
                    "2. Ubah data reservasi",
                    "3. Hapus data reservasi"
                }
            });
        }

        // 7. Menu customer setelah memilih role customer
        // GET /api/auth/customer-menu
        [HttpGet("customer-menu")]
        public IActionResult GetCustomerMenu()
        {
            return Ok(new
            {
                message = "Silakan pilih menu customer.",
                menu = new[]
                {
                    "1. Register",
                    "2. Login"
                }
            });
        }

        // 8. Menampilkan pilihan jam tersedia setelah customer login berhasil
        // GET /api/auth/available-slots
        [HttpGet("available-slots")]
        public IActionResult GetAvailableSlots()
        {
            return Ok(new
            {
                message = "Berikut pilihan jam yang tersedia.",
                slots = new[]
                {
                    "1. 08.00-09.00",
                    "2. 09.00-10.00",
                    "3. 10.00-11.00",
                    "4. 11.00-12.00",
                    "5. 13.00-14.00",
                    "6. 14.00-15.00",
                    "7. 15.00-16.00"
                }
            });
        }
    }
}