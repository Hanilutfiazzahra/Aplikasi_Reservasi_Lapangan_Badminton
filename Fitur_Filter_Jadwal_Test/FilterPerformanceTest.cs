using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Aplikasi_Reservasi_Lapangan_Badminton.Entities;
using Aplikasi_Reservasi_Lapangan_Badminton.Services;

namespace Fitur_Filter_Jadwal_Test
{
    [TestClass]
    public class FilterPerformanceTest
    {
        [TestMethod]
        public void PerformanceTest_FilterData()
        {
            FilterService filterService = new FilterService();

            List<Lapangan> daftarLapangan = new List<Lapangan>();

            for (int i = 0; i < 10000; i++)
            {
                if (i % 2 == 0)
                {
                    LapanganRegular lapRegular = new LapanganRegular("R00" + i,"Court Regular " + i,"Gedung A",50000);

                    lapRegular.jadwal.Add( "08:00 - 09:00", false);

                    daftarLapangan.Add( lapRegular);
                }

                else
                {
                    LapanganVIP lapVIP= new LapanganVIP("V00" + i, "Court VIP " + i,"Gedung B",80000, new string[] {"AC","LED","Sofa" });

                    lapVIP.jadwal.Add("08:00 - 09:00", false);

                    daftarLapangan.Add(lapVIP);
                }
            }

            Stopwatch stopwatch= new Stopwatch();

            stopwatch.Start();

            var hasil = filterService.FilterData( daftarLapangan,l => l.jadwal.ContainsKey("08:00 - 09:00"));

            stopwatch.Stop();

            Console.WriteLine( "Waktu Eksekusi: " + stopwatch.ElapsedMilliseconds + " ms");
            Assert.IsTrue( stopwatch.ElapsedMilliseconds < 1000);
        }
    }
}