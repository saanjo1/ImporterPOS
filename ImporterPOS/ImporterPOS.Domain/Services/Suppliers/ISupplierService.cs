using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.SearchObjects;
using ImporterPOS.Domain.Services.Generic;

namespace ImporterPOS.Domain.Services.Suppliers
{
    public interface ISupplierService : ICRUDService<Supplier, SupplierSearchObject>
    {
    }
}
