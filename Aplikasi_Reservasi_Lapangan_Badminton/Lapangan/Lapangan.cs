using System;
using System.Collections.Generic;
using System.Text;

namespace Aplikasi_Reservasi_Lapangan_Badminton.Entities
{
    public class Lapangan
    {

        public string id;

        public string nama;

        public string lokasi;

        public double hargaPerJam;

        public string tipe;

        public Dictionary<string, bool> jadwal
            = new Dictionary<string, bool>();

        public Lapangan(
            string id,
            string nama,
            string lokasi,
            double hargaPerJam,
            string tipe
        )
        {

            this.id = id;

            this.nama = nama;

            this.lokasi = lokasi;

            this.hargaPerJam = hargaPerJam;

            this.tipe = tipe;
        }

        public virtual string getDetail()
        {
            return nama
                + " ("
                + tipe
                + ") - "
                + lokasi
                + " - Rp"
                + hargaPerJam
                + "/jam";
        }
    }
}
