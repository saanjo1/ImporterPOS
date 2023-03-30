using ImporterPOS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.InventoryDocuments
{
    public class InventoryDocumentsService : IInventoryDocumentsService
    {
        public Task<bool> CreateInventoryDocAsync(InventoryDocument inventoryDocument)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteInventoryDocAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InventoryDocument>> GetAllInventoryDocsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Supplier> GetInventoryDocByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateInventoryDocAsync(InventoryDocument inventoryDocument)
        {
            throw new NotImplementedException();
        }
    }
}
