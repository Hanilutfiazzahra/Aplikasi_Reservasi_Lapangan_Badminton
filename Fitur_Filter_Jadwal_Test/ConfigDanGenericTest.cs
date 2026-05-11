using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aplikasi_Reservasi_Lapangan_Badminton.Ravie;

namespace Fitur_Filter_Jadwal_Test
{
    [TestClass]
    public class ConfigDanGenericTest
    {
        [TestMethod]
        public void TestConfig()
        {
            ConfigService config = new ConfigService();
            HargaService harga = new HargaService(config);

            decimal hasil = harga.HitungHarga(100000);

            Assert.IsTrue(hasil > 0);
        }

        [TestMethod]
        public void TestGeneric()
        {
            GenericRepository<string> repo =
                new GenericRepository<string>();

            repo.Add("Booking");

            Assert.AreEqual(1, repo.GetAll().Count);
        }
    }
}
