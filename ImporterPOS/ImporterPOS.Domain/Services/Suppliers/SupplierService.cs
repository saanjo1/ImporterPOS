using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Suppliers
{
    public class SupplierService : ISupplierService
    {

        private readonly IRepository<Supplier> _supplierRepository;

        public SupplierService(IRepository<Supplier> supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }

        public async Task<bool> CreateSupplierAsync(Supplier supplier)
        {
            try
            {
                await _supplierRepository.AddAsync(supplier);
                return true;
            }
            catch
            {
                return false;
                throw;
            }       
        
        }

        public async Task<bool> DeleteSupplierAsync(string id)
        {
            try
            {
                await _supplierRepository.DeleteAsync(id);
                return true;
            }
            catch
            {
                return false;
            }      
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            return await _supplierRepository.GetAllAsync();
        }

        public async Task<Supplier> GetSupplierByIdAsync(string id)
        {
            return await _supplierRepository.GetByIdAsync(id);
        }

        public async Task<Guid> GetSupplierByName(string name)
        {
            try
            {
                // Use the generic repository to get the Supplier by name
                var supplier = await _supplierRepository.GetByNameAsync(name, 0);

                if (supplier == null)
                {
                    // Create a new Supplier with the specified name and add it to the database
                    supplier = new Supplier {Id = Guid.NewGuid(), Name = name, IsDeleted = false };
                    await _supplierRepository.AddAsync(supplier);
                }

                return supplier.Id;
            }
            catch
            {
                return Guid.Empty;
            }
        }

        public async Task<bool> UpdateSupplierAsync(Supplier supplier)
        {
            try
            {
                await _supplierRepository.UpdateAsync(supplier);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
