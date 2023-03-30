using ImporterPOS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.InventoryDocuments
{
    public interface IInventoryDocumentsService
    {
        Task<IEnumerable<InventoryDocument>> GetAllInventoryDocsAsync();
        Task<InventoryDocument> GetInventoryDocByIdAsync(string id);
        Task<bool> CreateInventoryDocAsync(InventoryDocument inventoryDocument);
        Task<bool> DeleteInventoryDocAsync(string id);
        Task<bool> UpdateInventoryDocAsync(InventoryDocument inventoryDocument);
        Task<int> GetInventoryOrderNumber();
    }
}
