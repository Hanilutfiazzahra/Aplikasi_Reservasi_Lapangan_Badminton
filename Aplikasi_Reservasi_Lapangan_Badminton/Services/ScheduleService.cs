using System;
using System.Collections.Generic;
using System.Text;
using Aplikasi_Reservasi_Lapangan_Badminton.Entities;

namespace Aplikasi_Reservasi_Lapangan_Badminton.Services
{
    public class ScheduleService
    {
        public void generateJadwal(
            Lapangan lapangan
        )
        {

            int[] jam =
            {
                8,9,10,11,12,13,14,
                15,16,17,18,19,20,21
            };

            if (jam.Length == 0)
            {
                throw new Exception(
                    "Data jam kosong."
                );
            }

            foreach (int start in jam)
            {

                int end = start + 1;

                string range =
                    $"{start:00}.00 - {end:00}.00";

                lapangan.jadwal[range]
                    = false;
            }

            if (
                lapangan.jadwal.Count == 0
            )
            {
                throw new Exception(
                    "Jadwal gagal dibuat."
                );
            }
        }
    }
}

