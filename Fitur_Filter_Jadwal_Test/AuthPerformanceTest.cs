using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Extensions.Options;
using Aplikasi_Reservasi_Lapangan_Badminton.Auth;

namespace Fitur_Filter_Jadwal_Test
{
    [TestClass]
    public class AuthPerformanceTest
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

        [TestMethod]
        public void Register_1000Users_ShouldCompleteUnder10Seconds()
        {
            var authService = CreateAuthService();

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 1; i <= 1000; i++)
            {
                authService.Register(new RegisterRequest
                {
                    Name = "User " + i,
                    Email = $"user{i}@gmail.com",
                    Password = "password123",
                    Role = "Customer"
                });
            }

            stopwatch.Stop();

            Console.WriteLine("Waktu register 1000 user: " + stopwatch.ElapsedMilliseconds + " ms");

            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 10000,
                "Proses register 1000 user terlalu lama.");
        }

        [TestMethod]
        public void Login_1000Times_ShouldCompleteUnder10Seconds()
        {
            var authService = CreateAuthService();

            authService.Register(new RegisterRequest
            {
                Name = "Budi",
                Email = "budi@gmail.com",
                Password = "password123",
                Role = "Customer"
            });

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 1; i <= 1000; i++)
            {
                authService.Login(new LoginRequest
                {
                    Email = "budi@gmail.com",
                    Password = "password123"
                });
            }

            stopwatch.Stop();

            Console.WriteLine("Waktu login 1000 kali: " + stopwatch.ElapsedMilliseconds + " ms");

            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 10000,
                "Proses login 1000 kali terlalu lama.");
        }

        [TestMethod]
        public void GetProfile_1000Times_ShouldCompleteUnder10Seconds()
        {
            var authService = CreateAuthService();

            var registerResult = authService.Register(new RegisterRequest
            {
                Name = "Budi",
                Email = "budi@gmail.com",
                Password = "password123",
                Role = "Customer"
            });

            Stopwatch stopwatch = Stopwatch.StartNew();

            for (int i = 1; i <= 1000; i++)
            {
                authService.GetProfile(registerResult.UserId);
            }

            stopwatch.Stop();

            Console.WriteLine("Waktu get profile 1000 kali: " + stopwatch.ElapsedMilliseconds + " ms");

            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 10000,
                "Proses get profile 1000 kali terlalu lama.");
        }
    }
}