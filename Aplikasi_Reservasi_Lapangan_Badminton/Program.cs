using System; 
using System.Collections.Generic; 
using System.Text; 
using Aplikasi_Reservasi_Lapangan_Badminton.Entities;
using Aplikasi_Reservasi_Lapangan_Badminton.Services; 

namespace Aplikasi_Reservasi_Lapangan_Badminton
{
    class Program
    {
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

            ScheduleService scheduleService
                = new ScheduleService();

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
        }
    }
}