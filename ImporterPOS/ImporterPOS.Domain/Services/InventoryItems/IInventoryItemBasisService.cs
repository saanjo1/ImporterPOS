using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.InventoryItems
{
    public interface IInventoryItemBasisService : BaseInterface<InventoryItemBasis>
    { 
        Task<ICollection<InventoryItemBasis>> GetItemsByInventoryId(string inventoryId);
    }
}
