using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Aplikasi_Reservasi_Lapangan_Badminton.Entities;
using Aplikasi_Reservasi_Lapangan_Badminton.Services;

namespace Fitur_Filter_Jadwal_Test
{
    [TestClass]
    public class FilterServiceTest
    {
        [TestMethod]
        public void FilterData_JamDitemukan_ReturnData()
        {
            FilterService filterService = new FilterService();
            List<Lapangan> daftarLapangan = new List<Lapangan>();

            LapanganRegular lap1 = new LapanganRegular("L001","Court A","Gedung A",50000);

            lap1.jadwal.Add("08:00 - 09:00", false);

            daftarLapangan.Add(lap1);

            var hasil = filterService.FilterData(daftarLapangan, l => l.jadwal.ContainsKey( "08:00 - 09:00"));

            Assert.AreEqual(1, hasil.Count);
        }

        [TestMethod]
        public void FilterData_VIP_ReturnData()
        {
            FilterService filterService= new FilterService();
            List<Lapangan> daftarLapangan = new List<Lapangan>();

            LapanganVIP lapVIP = new LapanganVIP("L002","Court VIP","Gedung B",80000, new string[]{"AC","LED","Sofa"});

            lapVIP.jadwal.Add( "10:00 - 11:00", false);

            daftarLapangan.Add(lapVIP);

            var hasil = filterService.FilterData(daftarLapangan,l => l.jadwal.ContainsKey( "10:00 - 11:00"));

            Assert.AreEqual(1, hasil.Count);
        }
    }
}