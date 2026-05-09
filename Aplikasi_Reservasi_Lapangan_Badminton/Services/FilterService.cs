using System;
using System.Collections.Generic;

namespace Aplikasi_Reservasi_Lapangan_Badminton.Services
{
    public class FilterService
    {
        public List<T> FilterData<T>(List<T> data, Func<T, bool> kondisi)
        {
            if (data == null)
            {
                throw new ArgumentNullException( "Data tidak boleh null");
            }

            List<T> hasil = new List<T>();

            foreach (T item in data)
            {
                if (kondisi(item))
                {
                    hasil.Add(item);
                }
            }
            return hasil;
        }
    }
}