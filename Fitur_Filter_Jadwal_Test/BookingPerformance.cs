using System;
using System.Diagnostics;
using Aplikasi_Reservasi_Lapangan_Badminton.Entities;
using Aplikasi_Reservasi_Lapangan_Badminton.Reservasi;
using Aplikasi_Reservasi_Lapangan_Badminton.Services;

namespace Fitur_Filter_Jadwal_Test
namespace Aplikasi_Reservasi_Lapangan_Badminton
{
    public class BookingPerformance
    {
        public static void Run()
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

            PaymentService payment =
                new PaymentService();

            Stopwatch stopwatch =
                new Stopwatch();

            stopwatch.Start();

            for (int i = 0; i < 100000; i++)
            {
                payment.hitungTotal(booking);
            }

            stopwatch.Stop();

            Console.WriteLine(
                "Waktu eksekusi booking: "
                + stopwatch.ElapsedMilliseconds
                + " ms"
            );
        }
    }
}