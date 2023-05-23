using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Goods
{
    public interface IGoodService : BaseInterface<Good>
    {
        Task<Guid> GetGoodByName(string name, bool stockCorrection);
        Task SetMainStockToZero();
        Task<decimal> SumQuantityOfGoodsById(Guid goodId, Guid storageId);
    }
}
