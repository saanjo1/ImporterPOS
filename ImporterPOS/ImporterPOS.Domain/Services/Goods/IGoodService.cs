using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Goods
{
    public interface IGoodService : BaseInterface<Good>
    {
        Task<Guid> GetGoodByName(string name);
    }
}
