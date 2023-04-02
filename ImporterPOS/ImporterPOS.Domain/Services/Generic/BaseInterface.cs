using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Generic
{
    public interface BaseInterface<T>
    {
        Task<ICollection<T>> GetAll();

        Task<T> Get(string id);

        Task<bool> Create(T entity);

        Task<T> Update(Guid id, T entity);

        Task<ICollection<T>> Delete(Guid id);
    }
}
