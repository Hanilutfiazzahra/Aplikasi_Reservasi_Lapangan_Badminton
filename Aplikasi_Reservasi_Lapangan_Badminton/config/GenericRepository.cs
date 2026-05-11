using System;
using System.Collections.Generic;
using System.Text;

namespace Aplikasi_Reservasi_Lapangan_Badminton.Ravie
{
    public class GenericRepository<T>
    {
        private List<T> items = new();

        public void Add(T item)
        {
            items.Add(item);
        }

        public List<T> GetAll()
        {
            return items;
        }
    }
}