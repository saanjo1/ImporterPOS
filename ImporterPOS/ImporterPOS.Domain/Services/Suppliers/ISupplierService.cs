using ImporterPOS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Suppliers
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<Supplier> GetSupplierByIdAsync(string id);
        Task<bool> CreateSupplierAsync(Supplier supplier);
        Task<bool> DeleteSupplierAsync(string id);
        Task<bool> UpdateSupplierAsync(Supplier supplier);
        
        Task<Guid> GetSupplierByName(string name); 
    }
}
