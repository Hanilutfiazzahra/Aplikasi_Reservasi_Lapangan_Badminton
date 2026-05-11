using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Aplikasi_Reservasi_Lapangan_Badminton.Entities;
using Aplikasi_Reservasi_Lapangan_Badminton.Services;

namespace Fitur_Filter_Jadwal_Test
{
    [TestClass]
    public class LapanganTest
    {
        // Test Lapangan Regular
        [TestMethod]
        public void TestLapanganRegular()
        {
            // Buat object Regular
            LapanganRegular lap =
                new LapanganRegular(
                    "L001",
                    "Court A",
                    "Gedung A",
                    50000
                );

            // Test tipe
            Assert.AreEqual(
                "Regular",
                lap.tipe
            );
        }

        // Test Lapangan VIP
        [TestMethod]
        public void TestLapanganVIP()
        {
            // Buat object VIP
            LapanganVIP lap =
                new LapanganVIP(
                    "L002",
                    "Court B",
                    "Gedung B",
                    80000,
                    new string[]
                    {
                        "AC",
                        "LED"
                    }
                );

            // Test tipe
            Assert.AreEqual(
                "VIP",
                lap.tipe
            );

            // Test jumlah fasilitas
            Assert.AreEqual(
                2,
                lap.fasilitas.Length
            );
        }
    }
}
