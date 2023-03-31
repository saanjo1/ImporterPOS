using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Goods
{
    public class GoodService : IGoodService
    {

        private readonly IRepository<Good> _goodsRepository;

        public GoodService(IRepository<Good> goodsRepository)
        {
            _goodsRepository = goodsRepository;
        }

        public async Task<bool> CreateGoodAsync(Good good)
        {
            try
            {
                await _goodsRepository.AddAsync(good);
                return true;
            }
            catch
            {
                return false;
                throw;
            }
        }

        public async Task<bool> DeleteGoodAsync(string id)
        {
            try
            {
                await _goodsRepository.DeleteAsync(id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Good>> GetAllGoodsAsync()
        {
            return await _goodsRepository.GetAllAsync();
        }

        public async Task<Good> GetGoodByIdAsync(string id)
        {
            return await _goodsRepository.GetByIdAsync(id);
        }

        public async Task<Guid> GetGoodByName(string name, int type = 0)
        {
            try
            {
                // Use the generic repository to get the entity by name
                var good = await _goodsRepository.GetByNameAsync(name, type);
                if(good.Id != null)
                    return good.Id;
                return Guid.Empty;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateGoodAsync(Good good)
        {
            try
            {
                await _goodsRepository.UpdateAsync(good);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
