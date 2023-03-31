using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.InventoryItems
{
    public class InventoryItemBasisService : IInventoryItemBasisService
    {
        private readonly IRepository<InventoryItemBasis> _invBasisRepository;

        public InventoryItemBasisService(IRepository<InventoryItemBasis> invBasisRepository)
        {
            _invBasisRepository = invBasisRepository;
        }

        public async Task<bool> CreateInventoryItemBasiscAsync(InventoryItemBasis invBasis)
        {
            try
            {
                await _invBasisRepository.AddAsync(invBasis);
                return true;
            }
            catch
            {
                return false;
                throw;
            }

        }

        public async Task<bool> DeleteInventoryItemBasisAsync(string id)
        {
            try
            {
                await _invBasisRepository.DeleteAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<InventoryItemBasis>> GetAllInventoryItemBasisAsync()
        {
            return await _invBasisRepository.GetAllAsync();
        }

        public async Task<InventoryItemBasis> GetInventoryItemBasisByIdAsync(string id)
        {
            return await _invBasisRepository.GetByIdAsync(id);
        }


        public async Task<bool> UpdateInventoryItemBasisAsync(InventoryItemBasis invBasis)
        {
            try
            {
                await _invBasisRepository.UpdateAsync(invBasis);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

