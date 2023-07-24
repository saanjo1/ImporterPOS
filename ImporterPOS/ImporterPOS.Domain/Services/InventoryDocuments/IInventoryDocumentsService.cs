using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.SearchObjects;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.InventoryDocuments
{
    public interface IInventoryDocumentsService : ICRUDService<InventoryDocument, InventoryDocumentSearchObject>
    {
        Task<int> GetInventoryOrderNumber();
    }
}
