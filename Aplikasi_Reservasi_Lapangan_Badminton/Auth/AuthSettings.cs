using System;
using System.Collections.Generic;
using System.Text;

namespace Aplikasi_Reservasi_Lapangan_Badminton.Auth
{
    public class AuthSettings
    {
        public int PasswordMinLength { get; set; }
        public int TokenExpirationMinutes { get; set; }
        public string[] AllowedRoles { get; set; } = Array.Empty<string>();
    }
}
