using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Aplikasi_Reservasi_Lapangan_Badminton.Entities;
using Aplikasi_Reservasi_Lapangan_Badminton.Services;
using Aplikasi_Reservasi_Lapangan_Badminton.Reservasi;
using Aplikasi_Reservasi_Lapangan_Badminton.Ravie;
using Aplikasi_Reservasi_Lapangan_Badminton.Auth;

namespace Aplikasi_Reservasi_Lapangan_Badminton
{
    class Program
    {
        static void Main(string[] args)
        {
            // ==============================
            // AUTH SETUP
            // ==============================
            var authSettings = Options.Create(new AuthSettings
            {
                PasswordMinLength = 8,
                TokenExpirationMinutes = 60,
                AllowedRoles = new[] { "Admin", "Customer" }
            });

            AuthService authService = new AuthService(authSettings);

            // Akun admin default
            try
            {
                authService.Register(new RegisterRequest
                {
                    Name = "Admin",
                    Email = "admin@gmail.com",
                    Password = "password123",
                    Role = "Admin"
                });
            }
            catch
            {
                // Jika akun admin sudah ada, program tetap lanjut
            }

            AuthResponse loginResult = JalankanAuth(authService);

            if (loginResult.Role == "Admin")
            {
                TampilkanMenuAdmin();
                return;
            }

            if (loginResult.Role == "Customer")
            {
                Console.Clear();
            }

            // ==============================
            // PROGRAM UTAMA CUSTOMER
            // Hanya muncul setelah customer berhasil login
            // ==============================

            List<Lapangan> daftarLapangan
                = new List<Lapangan>();

            LapanganRegular lap1
                = new LapanganRegular(
                    "L001",
                    "Court A",
                    "Gedung A",
                    50000
                );

            LapanganVIP lap2
                = new LapanganVIP(
                    "L002",
                    "Court B",
                    "Gedung B",
                    80000,
                    new string[]
                    {
                        "AC",
                        "LED",
                        "Sofa"
                    }
                );

            daftarLapangan.Add(lap1);
            daftarLapangan.Add(lap2);

            ScheduleService scheduleService = new ScheduleService();

            // filter
            FilterService filterService = new FilterService();

            foreach (Lapangan lapangan in daftarLapangan)
            {
                scheduleService.generateJadwal(lapangan);
            }

            Console.WriteLine(
                "=== DAFTAR LAPANGAN ==="
            );

            Console.WriteLine();

            for (int i = 0; i < daftarLapangan.Count; i++)
            {
                Lapangan lap = daftarLapangan[i];

                Console.WriteLine((i + 1) + ". " + lap.getDetail());

                foreach (var item in lap.jadwal)
                {
                    Console.WriteLine("   " + item.Key + " " + (item.Value ? "(Booked)" : "(Tersedia)"));
                }

                Console.WriteLine();
            }

            Console.WriteLine("Data lapangan berhasil ditampilkan");

            // filter
            Console.WriteLine();
            Console.WriteLine("=== FILTER JADWAL ===");

            Console.WriteLine("Masukkan jam yang ingin dicari:");

            string inputJam = Console.ReadLine();

            var hasilFilter = filterService.FilterData(daftarLapangan, l => l.jadwal.ContainsKey(inputJam));

            Console.WriteLine();

            Console.WriteLine("=== HASIL FILTER ===");

            if (hasilFilter.Count == 0)
            {
                Console.WriteLine(
                    "Jadwal tidak ditemukan"
                );
            }

            foreach (var lapangan in hasilFilter)
            {
                bool status = lapangan.jadwal[inputJam];

                Console.WriteLine(lapangan.getDetail() + " | Status: " + (status ? "Booked" : "Tersedia"));
            }

            // Reservasi
            Console.WriteLine("\n==============================");
            Console.WriteLine("    MENU BOOKING & PAYMENT    ");
            Console.WriteLine("==============================");

            // 1. Customer
            Console.Write("Masukkan nama customer : ");
            string namaCustomer = Console.ReadLine();

            try
            {
                // 2. Pilih Lapangan
                Console.WriteLine("\nDaftar Lapangan:");
                for (int i = 0; i < daftarLapangan.Count; i++)
                {
                    Console.WriteLine((i + 1) + ". " + daftarLapangan[i].nama + " - " + daftarLapangan[i].lokasi);
                }

                Console.Write("Masukkan nomor lapangan : ");
                int pilihLapangan = Convert.ToInt32(Console.ReadLine());

                // Validasi index
                if (pilihLapangan < 1 || pilihLapangan > daftarLapangan.Count)
                    throw new Exception("Nomor lapangan tidak tersedia!");

                Lapangan lapanganDipilih = daftarLapangan[pilihLapangan - 1];

                // 3. Pilih Jadwal
                Console.WriteLine("\nJadwal Tersedia di " + lapanganDipilih.nama + ":");
                foreach (var item in lapanganDipilih.jadwal)
                {
                    string statusJadwal = item.Value ? "[Booked]" : "[Tersedia]";
                    Console.WriteLine("- " + item.Key + " " + statusJadwal);
                }

                Console.Write("Masukkan jadwal (Ketik jamnya) : ");
                string jadwal = Console.ReadLine();

                // 4. Durasi
                Console.Write("Masukkan durasi (jam) : ");
                int durasi = Convert.ToInt32(Console.ReadLine());

                // 5. Booking
                Booking booking1 = new Booking(
                    namaCustomer,
                    lapanganDipilih,
                    jadwal,
                    durasi
                );

                // runtime config
                ConfigService config = new ConfigService();
                HargaService hargaService = new HargaService(config);

                decimal hargaAwal = (decimal)(lapanganDipilih.hargaPerJam * durasi);

                decimal hargaFinal = hargaService.HitungHarga(hargaAwal);

                Console.WriteLine("\n=== RUNTIME CONFIG ===");
                Console.WriteLine("Harga Awal  : Rp" + hargaAwal);
                Console.WriteLine("Harga Final : Rp" + hargaFinal);

                // generic class
                GenericRepository<Booking> bookingRepo =
                    new GenericRepository<Booking>();

                bookingRepo.Add(booking1);

                Console.WriteLine("\n=== DATA BOOKING (GENERIC) ===");

                foreach (var booking in bookingRepo.GetAll())
                {
                    Console.WriteLine(
                        booking.namaCustomer +
                        " | " +
                        booking.lapangan.nama +
                        " | " +
                        booking.status
                    );
                }

                // 6. Hitung Pembayaran
                PaymentService paymentService = new PaymentService();
                double total = paymentService.hitungTotal(booking1);

                // 7. Tampilkan Detail Sebelum Bayar
                Console.WriteLine("\n=== DETAIL BOOKING ===");
                Console.WriteLine("Customer    : " + booking1.namaCustomer);
                Console.WriteLine("Lapangan    : " + booking1.lapangan.nama);
                Console.WriteLine("Jadwal      : " + booking1.jadwal);
                Console.WriteLine("Durasi      : " + booking1.durasi + " jam");
                Console.WriteLine("Total Bayar : Rp" + total);

                // 8. Pembayaran
                Console.Write("\nKonfirmasi bayar sekarang? (y/n): ");
                string konfirmasi = Console.ReadLine();

                if (konfirmasi.ToLower() == "y")
                {
                    booking1.Bayar();
                    Console.WriteLine("\nPembayaran Berhasil!");

                    Console.WriteLine("\n=== DAFTAR LAPANGAN TERBARU (SETELAH BOOKING) ===");
                    for (int i = 0; i < daftarLapangan.Count; i++)
                    {
                        Lapangan lap = daftarLapangan[i];
                        Console.WriteLine((i + 1) + ". " + lap.getDetail());

                        foreach (var item in lap.jadwal)
                        {
                            string statusTeks = item.Value ? "(Booked oleh " + namaCustomer + ")" : "(Tersedia)";
                            Console.WriteLine("   " + item.Key + " " + statusTeks);
                        }

                        Console.WriteLine();
                    }

                    Console.WriteLine("\n=== UPDATE STATUS JADWAL ===");
                    Console.WriteLine("Jadwal " + jadwal + " di " + lapanganDipilih.nama + " sekarang: [BOOKED oleh " + namaCustomer + "]");
                }
                else
                {
                    Console.WriteLine("\nPembayaran ditunda.");
                }

                Console.WriteLine("Status Booking Akhir : " + booking1.status);
            }
            catch (Exception ex)
            {
                Console.WriteLine("\n[ERROR]: " + ex.Message);
            }
        }

        static AuthResponse JalankanAuth(AuthService authService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=================================");
                Console.WriteLine(" SISTEM RESERVASI LAPANGAN");
                Console.WriteLine("=================================");
                Console.WriteLine("Silakan pilih role:");
                Console.WriteLine("1. Admin");
                Console.WriteLine("2. Customer");
                Console.Write("Masukkan pilihan role: ");

                string pilihanRole = Console.ReadLine();

                if (pilihanRole == "1")
                {
                    return LoginAdmin(authService);
                }
                else if (pilihanRole == "2")
                {
                    return MenuCustomer(authService);
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("[ERROR] Pilihan tidak valid. Masukkan 1 untuk Admin atau 2 untuk Customer.");
                    Console.WriteLine("Tekan Enter untuk mencoba lagi...");
                    Console.ReadLine();
                }
            }
        }

        static AuthResponse LoginAdmin(AuthService authService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=================================");
                Console.WriteLine(" LOGIN ADMIN");
                Console.WriteLine("=================================");

                Console.Write("Email    : ");
                string email = Console.ReadLine();

                Console.Write("Password : ");
                string password = Console.ReadLine();

                try
                {
                    AuthResponse result = authService.Login(new LoginRequest
                    {
                        Email = email,
                        Password = password
                    });

                    if (result.Role != "Admin")
                    {
                        throw new Exception("Akun ini bukan admin.");
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("[ERROR] " + ex.Message);
                    Console.WriteLine("Silakan masukkan data admin lagi.");
                    Console.WriteLine("Tekan Enter untuk mencoba lagi...");
                    Console.ReadLine();
                }
            }
        }

        static void TampilkanMenuAdmin()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=================================");
                Console.WriteLine(" MENU ADMIN");
                Console.WriteLine("=================================");
                Console.WriteLine("1. Lihat data reservasi");
                Console.WriteLine("2. Ubah data reservasi");
                Console.WriteLine("3. Hapus data reservasi");
                Console.Write("Pilih menu admin: ");

                string pilihan = Console.ReadLine();

                if (pilihan == "1")
                {
                    Console.WriteLine();
                    Console.WriteLine("Menu Lihat Data Reservasi dipilih.");
                    break;
                }
                else if (pilihan == "2")
                {
                    Console.WriteLine();
                    Console.WriteLine("Menu Ubah Data Reservasi dipilih.");
                    break;
                }
                else if (pilihan == "3")
                {
                    Console.WriteLine();
                    Console.WriteLine("Menu Hapus Data Reservasi dipilih.");
                    break;
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("[ERROR] Pilihan menu admin tidak valid.");
                    Console.WriteLine("Tekan Enter untuk mencoba lagi...");
                    Console.ReadLine();
                }
            }

            Console.WriteLine();
            Console.WriteLine("Tekan Enter untuk keluar...");
            Console.ReadLine();
        }

        static AuthResponse MenuCustomer(AuthService authService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=================================");
                Console.WriteLine(" MENU CUSTOMER");
                Console.WriteLine("=================================");
                Console.WriteLine("1. Register");
                Console.WriteLine("2. Login");
                Console.Write("Masukkan pilihan: ");

                string pilihan = Console.ReadLine();

                if (pilihan == "1")
                {
                    RegisterCustomer(authService);

                    Console.WriteLine();
                    Console.WriteLine("Silakan login menggunakan akun customer yang baru dibuat.");
                    Console.WriteLine("Tekan Enter untuk lanjut ke menu login...");
                    Console.ReadLine();

                    return LoginCustomer(authService);
                }
                else if (pilihan == "2")
                {
                    return LoginCustomer(authService);
                }
                else
                {
                    Console.WriteLine();
                    Console.WriteLine("[ERROR] Pilihan tidak valid. Masukkan 1 untuk Register atau 2 untuk Login.");
                    Console.WriteLine("Tekan Enter untuk mencoba lagi...");
                    Console.ReadLine();
                }
            }
        }

        static void RegisterCustomer(AuthService authService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=================================");
                Console.WriteLine(" REGISTER CUSTOMER");
                Console.WriteLine("=================================");

                Console.Write("Nama     : ");
                string name = Console.ReadLine();

                Console.Write("Email    : ");
                string email = Console.ReadLine();

                Console.Write("Password : ");
                string password = Console.ReadLine();

                try
                {
                    authService.Register(new RegisterRequest
                    {
                        Name = name,
                        Email = email,
                        Password = password,
                        Role = "Customer"
                    });

                    Console.WriteLine();
                    Console.WriteLine("Register customer berhasil.");
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("[ERROR] " + ex.Message);
                    Console.WriteLine("Silakan masukkan data register lagi.");
                    Console.WriteLine("Tekan Enter untuk mencoba lagi...");
                    Console.ReadLine();
                }
            }
        }

        static AuthResponse LoginCustomer(AuthService authService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=================================");
                Console.WriteLine(" LOGIN CUSTOMER");
                Console.WriteLine("=================================");

                Console.Write("Email    : ");
                string email = Console.ReadLine();

                Console.Write("Password : ");
                string password = Console.ReadLine();

                try
                {
                    AuthResponse result = authService.Login(new LoginRequest
                    {
                        Email = email,
                        Password = password
                    });

                    if (result.Role != "Customer")
                    {
                        throw new Exception("Akun ini bukan customer.");
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine();
                    Console.WriteLine("[ERROR] " + ex.Message);
                    Console.WriteLine("Silakan masukkan data login lagi.");
                    Console.WriteLine("Tekan Enter untuk mencoba lagi...");
                    Console.ReadLine();
                }
            }
        }
    }
}