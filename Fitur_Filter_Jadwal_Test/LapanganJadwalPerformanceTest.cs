using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Aplikasi_Reservasi_Lapangan_Badminton.Entities;
using Aplikasi_Reservasi_Lapangan_Badminton.Services;

namespace Fitur_Filter_Jadwal_Test
{
    [TestClass]
    public class LapanganJadwalPerformanceTest
    {
        [TestMethod]
        public void TestFullPerformance()
        {
            // Stopwatch hitung waktu
            Stopwatch stopwatch
                = new Stopwatch();

            // Start timer
            stopwatch.Start();

            // List lapangan
            List<Lapangan> daftarLapangan
                = new List<Lapangan>();

            // Service generate jadwal
            ScheduleService service
                = new ScheduleService();

            // Generate banyak data
            for (int i = 0; i < 1000; i++)
            {
                // Lapangan Regular
                LapanganRegular regular
                    = new LapanganRegular(
                        "R" + i,
                        "Regular " + i,
                        "Gedung A",
                        50000
                    );

                // Generate jadwal
                service.generateJadwal(
                    regular
                );

                // Simpan ke list
                daftarLapangan.Add(
                    regular
                );

                // Lapangan VIP
                LapanganVIP vip
                    = new LapanganVIP(
                        "V" + i,
                        "VIP " + i,
                        "Gedung B",
                        80000,
                        new string[]
                        {
                            "AC",
                            "LED"
                        }
                    );

                // Generate jadwal
                service.generateJadwal(
                    vip
                );

                // Simpan ke list
                daftarLapangan.Add(
                    vip
                );
            }

            // Stop timer
            stopwatch.Stop();

            // Test maksimal 3 detik
            Assert.IsTrue(
                stopwatch.ElapsedMilliseconds
                < 3000
            );
        }
    }
}
