using System;
using System.Collections.Generic;
using Aplikasi_Reservasi_Lapangan_Badminton.Entities;
using Aplikasi_Reservasi_Lapangan_Badminton.Reservasi;

namespace Aplikasi_Reservasi_Lapangan_Badminton.Services
{
    public class PaymentService
    {
        public double hitungTotal(Booking booking)
        {
            if (booking == null) return 0;
            return booking.lapangan.hargaPerJam * booking.durasi;
        }
    }
}