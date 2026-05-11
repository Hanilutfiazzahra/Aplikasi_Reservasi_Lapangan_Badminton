using Microsoft.Extensions.Options;

namespace Aplikasi_Reservasi_Lapangan_Badminton.Auth
{
    public class AuthService
    {
        private readonly AuthSettings _authSettings;
        private readonly List<UserAccount> _users = new();

        private int _nextUserId = 1;

        public AuthService(IOptions<AuthSettings> authSettings)
        {
            _authSettings = authSettings.Value;
        }

        public string[] GetAllowedRoles()
        {
            // DbC: Postcondition
            Ensure(_authSettings.AllowedRoles.Length > 0, "Daftar role tidak boleh kosong");

            return _authSettings.AllowedRoles;
        }

        public AuthResponse Register(RegisterRequest? request)
        {
            // DbC: Precondition
            Require(request != null, "Data register tidak boleh kosong");
            Require(!string.IsNullOrWhiteSpace(request!.Name), "Nama tidak boleh kosong");
            Require(IsValidEmail(request.Email), "Format email tidak valid");
            Require(!string.IsNullOrWhiteSpace(request.Password), "Password tidak boleh kosong");
            Require(request.Password.Length >= _authSettings.PasswordMinLength,
                $"Password minimal {_authSettings.PasswordMinLength} karakter");
            Require(_authSettings.AllowedRoles.Contains(request.Role), "Role tidak diperbolehkan");
            Require(!_users.Any(u => u.Email == request.Email), "Email sudah terdaftar");

            var user = new UserAccount
            {
                Id = _nextUserId++,
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                Role = request.Role
            };

            _users.Add(user);

            // DbC: Postcondition
            Ensure(user.Id > 0, "User ID harus terbentuk");
            Ensure(_users.Contains(user), "User harus berhasil tersimpan");

            return new AuthResponse
            {
                Message = "Register berhasil",
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                Token = GenerateToken(),
                TokenExpiredAt = DateTime.Now.AddMinutes(_authSettings.TokenExpirationMinutes)
            };
        }

        public AuthResponse Login(LoginRequest? request)
        {
            // DbC: Precondition
            Require(request != null, "Data login tidak boleh kosong");
            Require(IsValidEmail(request!.Email), "Format email tidak valid");
            Require(!string.IsNullOrWhiteSpace(request.Password), "Password tidak boleh kosong");

            var user = _users.FirstOrDefault(u => u.Email == request.Email);

            Require(user != null, "Email tidak terdaftar");
            Require(user!.Password == request.Password, "Password salah");

            var response = new AuthResponse
            {
                Message = "Login berhasil",
                UserId = user.Id,
                Email = user.Email,
                Role = user.Role,
                Token = GenerateToken(),
                TokenExpiredAt = DateTime.Now.AddMinutes(_authSettings.TokenExpirationMinutes)
            };

            // DbC: Postcondition
            Ensure(response.UserId > 0, "User ID harus dikembalikan");
            Ensure(!string.IsNullOrWhiteSpace(response.Token), "Token harus terbentuk");

            return response;
        }

        public UserAccount GetProfile(int id)
        {
            // DbC: Precondition
            Require(id > 0, "ID user tidak valid");

            var user = _users.FirstOrDefault(u => u.Id == id);

            Require(user != null, "User tidak ditemukan");

            // DbC: Postcondition
            Ensure(user!.Id == id, "Data user harus sesuai ID");

            return user;
        }

        private static bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && email.Contains("@");
        }

        private static string GenerateToken()
        {
            return Guid.NewGuid().ToString();
        }

        private static void Require(bool condition, string message)
        {
            if (!condition)
                throw new ArgumentException(message);
        }

        private static void Ensure(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }
    }
}