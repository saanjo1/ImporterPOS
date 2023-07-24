using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Generic
{
    public interface ICRUDService<T, TSearch> where T : class where TSearch : class
    {
        ICollection<T> Get(TSearch search = null);

        T GetById(Guid id);

        T Create(T entity);

        T Update(Guid id, T entity);

        ICollection<T> Delete(Guid id);
    }
}
