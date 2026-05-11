using System;
using System.Collections.Generic;
using System.Text;

namespace Aplikasi_Reservasi_Lapangan_Badminton.Ravie
{
    public class HargaService
    {
        private readonly ConfigService _config;

        public HargaService(ConfigService config)
        {
            _config = config;
        }

        public decimal HitungHarga(decimal hargaAwal)
        {
            if (hargaAwal < 0)
            {
                throw new Exception("Harga tidak boleh negatif");
            }

            decimal discount = _config.GetDiscount();
            decimal tax = _config.GetTax();

            decimal hasil = hargaAwal - (hargaAwal * discount);
            hasil += hasil * tax;

            return hasil;
        }
    }
}