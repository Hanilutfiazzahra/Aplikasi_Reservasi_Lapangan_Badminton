using System;
using Aplikasi_Reservasi_Lapangan_Badminton.Entities;

namespace Aplikasi_Reservasi_Lapangan_Badminton.Reservasi
{
	public class Booking
	{
		public string namaCustomer;
		public Lapangan lapangan;
		public string jadwal;
		public int durasi;
		public BookingState status;

		public Booking(string namaCustomer, Lapangan lapangan, string jadwal, int durasi)
		{
			// --- Defensive Programming / Design by Contract ---
			if (string.IsNullOrWhiteSpace(namaCustomer))
			{
				throw new ArgumentException("Nama customer tidak boleh kosong!");
			}
			if (lapangan == null)
			{
				throw new ArgumentNullException("Lapangan harus dipilih!");
			}
			if (durasi <= 0)
			{
				throw new ArgumentOutOfRangeException("Durasi minimal adalah 1 jam!");
			}

			this.namaCustomer = namaCustomer;
			this.lapangan = lapangan;
			this.jadwal = jadwal;
			this.durasi = durasi;

			// automata
			this.status = BookingState.Pending;
		}

		public void Bayar()
		{
			if (this.status == BookingState.Pending)
			{
				this.status = BookingState.Paid;

				string teksJam = this.jadwal[0].ToString() + this.jadwal[1].ToString();
				int jamMulai = int.Parse(teksJam);

				for (int i = 0; i < durasi; i++)
				{
					int jamSekarang = jamMulai + i;
					int jamSelesai = jamSekarang + 1;

					// Jam
					string jamFormat;
					if (jamSekarang < 10)
					{
						jamFormat = "0" + jamSekarang + ".00";
					}
					else
					{
						jamFormat = jamSekarang + ".00";
					}

					string selesaiFormat;
					if (jamSelesai < 10)
					{
						selesaiFormat = "0" + jamSelesai + ".00";
					}
					else
					{
						selesaiFormat = jamSelesai + ".00";
					}

					// Dictionary
					string keyJadwal = jamFormat + " - " + selesaiFormat;

					// Tandai lapangan jadi sudah dipesan
					if (this.lapangan.jadwal.ContainsKey(keyJadwal))
					{
						this.lapangan.jadwal[keyJadwal] = true;
					}
				}
			}
		}
	}
}