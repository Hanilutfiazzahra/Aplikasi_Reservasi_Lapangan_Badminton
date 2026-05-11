using System; 
using System.Collections.Generic; 
using System.Text;


using Aplikasi_Reservasi_Lapangan_Badminton.Entities; 
using Aplikasi_Reservasi_Lapangan_Badminton.Services;
using Aplikasi_Reservasi_Lapangan_Badminton.Reservasi;
using Aplikasi_Reservasi_Lapangan_Badminton.Ravie;



namespace Aplikasi_Reservasi_Lapangan_Badminton
{
    class Program{

        static void Main(string[] args)
        {

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

            //filter
            FilterService filterService = new FilterService();

            foreach (Lapangan lapangan
                in daftarLapangan)
            {
                scheduleService.generateJadwal(lapangan);
            }

            Console.WriteLine(
                "=== DAFTAR LAPANGAN ==="
            );

            Console.WriteLine();

            for (int i = 0; i < daftarLapangan.Count;i++)
            {
                Lapangan lap = daftarLapangan[i];

                Console.WriteLine ((i + 1) + ". " + lap.getDetail());

                foreach (var item in lap.jadwal)
                {
                    Console.WriteLine( "   " + item.Key + " " + (item.Value? "(Booked)" : "(Tersedia)"));
                }

                Console.WriteLine();
            }

            Console.WriteLine("Data lapangan berhasil ditampilkan");

            //filter
            Console.WriteLine();
            Console.WriteLine("=== FILTER JADWAL ===");

            Console.WriteLine("Masukkan jam yang ingin dicari:");

            string inputJam = Console.ReadLine();

            var hasilFilter = filterService.FilterData(daftarLapangan,l => l.jadwal.ContainsKey(inputJam));

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

                Console.WriteLine(lapangan.getDetail() + " | Status: " + ( status? "Booked": "Tersedia"));
            }

            //Reservasi
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

                // Validasi index (Defensive Programming)
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

                //runtime config
                ConfigService config = new ConfigService();
                HargaService hargaService = new HargaService(config);

                decimal hargaAwal = (decimal)(lapanganDipilih.hargaPerJam * durasi);

                decimal hargaFinal = hargaService.HitungHarga(hargaAwal);

                Console.WriteLine("\n=== RUNTIME CONFIG ===");
                Console.WriteLine("Harga Awal  : Rp" + hargaAwal);
                Console.WriteLine("Harga Final : Rp" + hargaFinal);

                //generic class
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
                            // Jika Value true, maka tampilkan Booked, jika false Tersedia
                            string statusTeks = item.Value ? "(Booked oleh " + namaCustomer + ")" : "(Tersedia)";
                            Console.WriteLine("   " + item.Key + " " + statusTeks);
                        }
                        Console.WriteLine();
                    }

                    // Output
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
                // Menangkap error jika input nomor lapangan atau durasi bukan angka
                Console.WriteLine("\n[ERROR]: " + ex.Message);
            }
        }
    }
}