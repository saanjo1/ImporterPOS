using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.InventoryDocuments
{
    public interface IInventoryDocumentsService : BaseInterface<InventoryDocument>
    {
        Task<int> GetInventoryOrderNumber();

        Task<decimal> GetTotalInventoryItems(string _documentId);
    }
}
