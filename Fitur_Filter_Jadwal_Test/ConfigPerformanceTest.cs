using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aplikasi_Reservasi_Lapangan_Badminton.Ravie;
using System.Diagnostics;

namespace Fitur_Filter_Jadwal_Test
{
    [TestClass]
    public class ConfigPerformanceTest
    {
        [TestMethod]
        public void TestPerformance()
        {
            Stopwatch stopwatch = new Stopwatch();

            ConfigService config = new ConfigService();
            HargaService harga = new HargaService(config);

            stopwatch.Start();

            for (int i = 0; i < 1000; i++)
            {
                harga.HitungHarga(100000);
            }

            stopwatch.Stop();

            Assert.IsTrue(stopwatch.ElapsedMilliseconds < 1000);
        }
    }
}
