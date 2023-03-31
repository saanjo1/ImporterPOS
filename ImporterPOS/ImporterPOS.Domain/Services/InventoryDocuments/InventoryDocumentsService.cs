using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.InventoryDocuments
{
    public class InventoryDocumentsService : IInventoryDocumentsService
    {
        private readonly IRepository<InventoryDocument> _invDocsRepository;

        public async Task<bool> CreateInventoryDocAsync(InventoryDocument inventoryDocument)
        {
            try
            {
                await _invDocsRepository.AddAsync(inventoryDocument);
                return true;
            }
            catch
            {
                return false;
                throw;
            }

        }

        public async Task<bool> DeleteInventoryDocAsync(string id)
        {
            try
            {
                await _invDocsRepository.DeleteAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<InventoryDocument>> GetAllInventoryDocsAsync()
        {
            return await _invDocsRepository.GetAllAsync();
        }

        public async Task<InventoryDocument> GetInventoryDocByIdAsync(string id)
        {
            return await _invDocsRepository.GetByIdAsync(id);
        }

        public async Task<int> GetInventoryOrderNumber()
        {
            return await _invDocsRepository.GetNumberOfRecords();
        }

        public async Task<bool> UpdateInventoryDocAsync(InventoryDocument invDoc)
        {
            try
            {
                await _invDocsRepository.UpdateAsync(invDoc);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
