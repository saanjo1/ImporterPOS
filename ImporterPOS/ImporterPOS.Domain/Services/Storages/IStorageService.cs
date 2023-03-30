using ImporterPOS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Storages
{
    public interface IStorageService
    {
        Task<IEnumerable<Storage>> GetAllStoragesAsync();
        Task<Storage> GetStorageByIdAsync(string id);
        Task<bool> CreateStorageDocAsync(Storage storage);
        Task<bool> DeleteStorageAsync(string id);
        Task<bool> UpdateStorageAsync(Storage storage);
        Task<Guid> GetSupplierByName(string name);
    }
}
