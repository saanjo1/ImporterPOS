using ImporterPOS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.InventoryItems
{
    public interface IInventoryItemBasisService
    {
        Task<IEnumerable<InventoryItemBasis>> GetAllInventoryItemBasisAsync();
        Task<InventoryItemBasis> GetInventoryItemBasisByIdAsync(string id);
        Task<bool> CreateInventoryItemBasiscAsync(InventoryItemBasis entity);
        Task<bool> DeleteInventoryItemBasisAsync(string id);
        Task<bool> UpdateInventoryItemBasisAsync(InventoryItemBasis entity);
    }
}
