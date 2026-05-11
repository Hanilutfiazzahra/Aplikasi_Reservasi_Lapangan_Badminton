using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aplikasi_Reservasi_Lapangan_Badminton.Entities;
using Aplikasi_Reservasi_Lapangan_Badminton.Reservasi;
using Aplikasi_Reservasi_Lapangan_Badminton.Services;
using System;

namespace Fitur_Filter_Jadwal_Test
{
    [TestClass]
    public class BookingTest
    {
        [TestMethod]
        public void TestBookingBayar()
        {
            LapanganRegular lapangan =
                new LapanganRegular(
                    "L001",
                    "Court A",
                    "Gedung A",
                    50000
                );

            lapangan.jadwal["08.00 - 09.00"] = false;

            Booking booking =
                new Booking(
                    "Ryan",
                    lapangan,
                    "08.00 - 09.00",
                    1
                );

            booking.Bayar();

            Assert.AreEqual(
                BookingState.Paid,
                booking.status
            );
        }

        [TestMethod]
        public void TestBookingCompleted()
        {
            LapanganRegular lapangan =
                new LapanganRegular(
                    "L001",
                    "Court A",
                    "Gedung A",
                    50000
                );

            lapangan.jadwal["08.00 - 09.00"] = false;

            Booking booking =
                new Booking(
                    "Ryan",
                    lapangan,
                    "08.00 - 09.00",
                    1
                );

            booking.Bayar();

            booking.SelesaikanBooking();

            Assert.AreEqual(
                BookingState.Completed,
                booking.status
            );
        }

        [TestMethod]
        public void TestHitungTotal()
        {
            LapanganRegular lapangan =
                new LapanganRegular(
                    "L001",
                    "Court A",
                    "Gedung A",
                    50000
                );

            Booking booking =
                new Booking(
                    "Ryan",
                    lapangan,
                    "08.00 - 09.00",
                    2
                );

            PaymentService payment =
                new PaymentService();

            double total =
                payment.hitungTotal(booking);

            Assert.AreEqual(
                100000,
                total
            );
        }
    }
}