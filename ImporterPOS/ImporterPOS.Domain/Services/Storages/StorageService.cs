using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Storages
{
    public class StorageService : IStorageService
    {
        private readonly IRepository<Storage> _storageRepository;

        public StorageService(IRepository<Storage> storageRepository)
        {
            _storageRepository = storageRepository;
        }

        public async Task<bool> CreateStorageDocAsync(Storage storage)
        {
            try
            {
                await _storageRepository.AddAsync(storage);
                return true;
            }
            catch
            {
                return false;
                throw;
            }
        }

        public async Task<bool> DeleteStorageAsync(string id)
        {
            try
            {
                await _storageRepository.DeleteAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Storage>> GetAllStoragesAsync()
        {
            return await _storageRepository.GetAllAsync();
        }

        public async Task<Storage> GetStorageByIdAsync(string id)
        {
            return await _storageRepository.GetByIdAsync(id);
        }

        public async Task<Guid> GetStorageByName(string name)
        {
            try
            {
                // Use the generic repository to get the Storage by name
                var storage = await _storageRepository.GetByNameAsync(name, 0);

                if (storage == null)
                {
                    // Create a new Storage with the specified name and add it to the database
                    storage = new Storage { Id = Guid.NewGuid(), Name = name, Deleted = false};
                    await _storageRepository.AddAsync(storage);
                }

                return storage.Id;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        public async Task<bool> UpdateStorageAsync(Storage storage)
        {
            try
            {
                await _storageRepository.UpdateAsync(storage);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
