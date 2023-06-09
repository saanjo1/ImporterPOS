using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Suppliers
{
    public interface ISupplierService : BaseInterface<Supplier>
    {
        Task<Guid> GetSupplierByName(string name); 
    }
}
