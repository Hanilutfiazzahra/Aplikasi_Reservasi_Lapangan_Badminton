using System;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aplikasi_Reservasi_Lapangan_Badminton.Auth;

namespace Fitur_Filter_Jadwal_Test
{
    [TestClass]
    public class AuthServiceTest
    {
        private AuthService CreateAuthService()
        {
            var settings = Options.Create(new AuthSettings
            {
                PasswordMinLength = 8,
                TokenExpirationMinutes = 60,
                AllowedRoles = new[] { "Admin", "Customer" }
            });

            return new AuthService(settings);
        }

        private void AssertArgumentException(Action action, string expectedMessage)
        {
            try
            {
                action();
                Assert.Fail("Seharusnya terjadi ArgumentException, tetapi tidak ada error yang muncul.");
            }
            catch (ArgumentException ex)
            {
                Assert.AreEqual(expectedMessage, ex.Message);
            }
        }

        [TestMethod]
        public void GetAllowedRoles_ShouldReturnConfiguredRoles()
        {
            var authService = CreateAuthService();

            var roles = authService.GetAllowedRoles();

            Assert.AreEqual(2, roles.Length);
            Assert.IsTrue(Array.Exists(roles, role => role == "Admin"));
            Assert.IsTrue(Array.Exists(roles, role => role == "Customer"));
        }

        [TestMethod]
        public void RegisterCustomer_WithValidData_ShouldSuccess()
        {
            var authService = CreateAuthService();

            var result = authService.Register(new RegisterRequest
            {
                Name = "Budi",
                Email = "budi@gmail.com",
                Password = "password123",
                Role = "Customer"
            });

            Assert.AreEqual("Register berhasil", result.Message);
            Assert.AreEqual("budi@gmail.com", result.Email);
            Assert.AreEqual("Customer", result.Role);
            Assert.IsTrue(result.UserId > 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Token));
        }

        [TestMethod]
        public void RegisterCustomer_WithEmptyName_ShouldShowError()
        {
            var authService = CreateAuthService();

            AssertArgumentException(() =>
            {
                authService.Register(new RegisterRequest
                {
                    Name = "",
                    Email = "budi@gmail.com",
                    Password = "password123",
                    Role = "Customer"
                });
            }, "Nama tidak boleh kosong");
        }

        [TestMethod]
        public void RegisterCustomer_WithInvalidEmail_ShouldShowError()
        {
            var authService = CreateAuthService();

            AssertArgumentException(() =>
            {
                authService.Register(new RegisterRequest
                {
                    Name = "Budi",
                    Email = "budigmail.com",
                    Password = "password123",
                    Role = "Customer"
                });
            }, "Format email tidak valid");
        }

        [TestMethod]
        public void RegisterCustomer_WithEmptyPassword_ShouldShowError()
        {
            var authService = CreateAuthService();

            AssertArgumentException(() =>
            {
                authService.Register(new RegisterRequest
                {
                    Name = "Budi",
                    Email = "budi@gmail.com",
                    Password = "",
                    Role = "Customer"
                });
            }, "Password tidak boleh kosong");
        }

        [TestMethod]
        public void RegisterCustomer_WithShortPassword_ShouldShowError()
        {
            var authService = CreateAuthService();

            AssertArgumentException(() =>
            {
                authService.Register(new RegisterRequest
                {
                    Name = "Budi",
                    Email = "budi@gmail.com",
                    Password = "123",
                    Role = "Customer"
                });
            }, "Password minimal 8 karakter");
        }

        [TestMethod]
        public void RegisterCustomer_WithInvalidRole_ShouldShowError()
        {
            var authService = CreateAuthService();

            AssertArgumentException(() =>
            {
                authService.Register(new RegisterRequest
                {
                    Name = "Budi",
                    Email = "budi@gmail.com",
                    Password = "password123",
                    Role = "Member"
                });
            }, "Role tidak diperbolehkan");
        }

        [TestMethod]
        public void RegisterCustomer_WithDuplicateEmail_ShouldShowError()
        {
            var authService = CreateAuthService();

            authService.Register(new RegisterRequest
            {
                Name = "Budi",
                Email = "budi@gmail.com",
                Password = "password123",
                Role = "Customer"
            });

            AssertArgumentException(() =>
            {
                authService.Register(new RegisterRequest
                {
                    Name = "Budi Kedua",
                    Email = "budi@gmail.com",
                    Password = "password123",
                    Role = "Customer"
                });
            }, "Email sudah terdaftar");
        }

        [TestMethod]
        public void LoginCustomer_WithValidData_ShouldSuccess()
        {
            var authService = CreateAuthService();

            authService.Register(new RegisterRequest
            {
                Name = "Budi",
                Email = "budi@gmail.com",
                Password = "password123",
                Role = "Customer"
            });

            var result = authService.Login(new LoginRequest
            {
                Email = "budi@gmail.com",
                Password = "password123"
            });

            Assert.AreEqual("Login berhasil", result.Message);
            Assert.AreEqual("budi@gmail.com", result.Email);
            Assert.AreEqual("Customer", result.Role);
            Assert.IsTrue(result.UserId > 0);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.Token));
        }

        [TestMethod]
        public void LoginCustomer_WithInvalidEmail_ShouldShowError()
        {
            var authService = CreateAuthService();

            AssertArgumentException(() =>
            {
                authService.Login(new LoginRequest
                {
                    Email = "budigmail.com",
                    Password = "password123"
                });
            }, "Format email tidak valid");
        }

        [TestMethod]
        public void LoginCustomer_WithEmptyPassword_ShouldShowError()
        {
            var authService = CreateAuthService();

            AssertArgumentException(() =>
            {
                authService.Login(new LoginRequest
                {
                    Email = "budi@gmail.com",
                    Password = ""
                });
            }, "Password tidak boleh kosong");
        }

        [TestMethod]
        public void LoginCustomer_WithUnregisteredEmail_ShouldShowError()
        {
            var authService = CreateAuthService();

            AssertArgumentException(() =>
            {
                authService.Login(new LoginRequest
                {
                    Email = "tidakada@gmail.com",
                    Password = "password123"
                });
            }, "Email tidak terdaftar");
        }

        [TestMethod]
        public void LoginCustomer_WithWrongPassword_ShouldShowError()
        {
            var authService = CreateAuthService();

            authService.Register(new RegisterRequest
            {
                Name = "Budi",
                Email = "budi@gmail.com",
                Password = "password123",
                Role = "Customer"
            });

            AssertArgumentException(() =>
            {
                authService.Login(new LoginRequest
                {
                    Email = "budi@gmail.com",
                    Password = "salah123"
                });
            }, "Password salah");
        }

        [TestMethod]
        public void GetProfile_WithValidUserId_ShouldReturnUser()
        {
            var authService = CreateAuthService();

            var registerResult = authService.Register(new RegisterRequest
            {
                Name = "Budi",
                Email = "budi@gmail.com",
                Password = "password123",
                Role = "Customer"
            });

            var profile = authService.GetProfile(registerResult.UserId);

            Assert.AreEqual(registerResult.UserId, profile.Id);
            Assert.AreEqual("Budi", profile.Name);
            Assert.AreEqual("budi@gmail.com", profile.Email);
            Assert.AreEqual("Customer", profile.Role);
        }

        [TestMethod]
        public void GetProfile_WithInvalidUserId_ShouldShowError()
        {
            var authService = CreateAuthService();

            AssertArgumentException(() =>
            {
                authService.GetProfile(0);
            }, "ID user tidak valid");
        }

        [TestMethod]
        public void GetProfile_WithUnknownUserId_ShouldShowError()
        {
            var authService = CreateAuthService();

            AssertArgumentException(() =>
            {
                authService.GetProfile(99);
            }, "User tidak ditemukan");
        }
    }
}