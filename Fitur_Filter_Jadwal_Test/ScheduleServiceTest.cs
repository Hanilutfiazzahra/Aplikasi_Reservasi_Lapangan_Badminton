using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Aplikasi_Reservasi_Lapangan_Badminton.Entities;
using Aplikasi_Reservasi_Lapangan_Badminton.Services;

namespace Fitur_Filter_Jadwal_Test
{
    [TestClass]
    public class ScheduleServiceTest
    {
        [TestMethod]
        public void TestGenerateJadwal()
        {
            // Buat lapangan
            LapanganRegular lap =
                new LapanganRegular(
                    "L001",
                    "Court A",
                    "Gedung A",
                    50000
                );

            // Buat service
            ScheduleService service
                = new ScheduleService();

            // Generate jadwal
            service.generateJadwal(lap);

            // Test jumlah slot
            Assert.AreEqual(
                14,
                lap.jadwal.Count
            );

            // Test status slot awal
            Assert.AreEqual(
                false,
                lap.jadwal["08.00 - 09.00"]
            );
        }
    }
}