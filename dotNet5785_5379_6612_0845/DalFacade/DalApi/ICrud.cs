using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DalApi
{
    public interface ICrud<T> where T : class
    {
        public void Create(T item);
        public void Delete(int id);
        public void DeleteAll();
        public T? Read(int id);
        IEnumerable<T> ReadAll(Func<T, bool>? filter = null);
        public void Update(T item);
    }
}

