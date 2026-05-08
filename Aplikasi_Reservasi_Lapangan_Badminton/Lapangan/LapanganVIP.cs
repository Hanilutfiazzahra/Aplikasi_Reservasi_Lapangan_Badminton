using System;
using System.Collections.Generic;
using System.Text;
using Aplikasi_Reservasi_Lapangan_Badminton.Entities;   

namespace Aplikasi_Reservasi_Lapangan_Badminton.Entities
{
    public class LapanganVIP : Lapangan
    {
        public string[] fasilitas;

        public LapanganVIP(
            string id,
            string nama,
            string lokasi,
            double harga,
            string[] fasilitas
        )

            : base(
                id,
                nama,
                lokasi,
                harga,
                "VIP"
            )
        {

            this.fasilitas = fasilitas;
        }

        public override string getDetail()
        {

            string fasilitasString
                = string.Join(", ", fasilitas);

            return base.getDetail()
                + " | Fasilitas: "
                + fasilitasString;
        }
    }
}
