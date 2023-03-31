using ImporterPOS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Goods
{
    public interface IGoodService
    {
        Task<IEnumerable<Good>> GetAllGoodsAsync();
        Task<Good> GetGoodByIdAsync(string id);
        Task<bool> CreateGoodAsync(Good good);
        Task<bool> DeleteGoodAsync(string id);
        Task<bool> UpdateGoodAsync(Good good);

        Task<Guid> GetGoodByName(string name, int type);
    }
}
