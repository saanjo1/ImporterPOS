using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.Services.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.IdentityModel.Tokens;
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
        private readonly DatabaseContextFactory _factory;

        public GoodService(DatabaseContextFactory factory)
        {
            _factory = factory;
        }

        public async Task<bool> Create(Good entity)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                try
                {
                    context.Add(entity);
                    await context.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public async Task<ICollection<Good>> Delete(Guid id)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Good? entity = await context.Goods.FirstOrDefaultAsync(x => x.Id == id);
                if (entity != null)
                    context.Remove(entity);

                context.SaveChangesAsync();
                ICollection<Good> entities = context.Goods.ToList();
                return entities;
            }
        }

        public async Task<Good> Get(string id)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Good? entity = await context.Goods.FirstOrDefaultAsync(x => x.Id.ToString() == id);
                return entity;
            }
        }

        public async Task<ICollection<Good>> GetAll()
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                ICollection<Good> entities = await context.Goods.ToListAsync();
                return entities;
            }
        }

        public async Task<Good> Update(Guid id, Good entity)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                entity.Id = id;
                context.Goods.Update(entity);
                await context.SaveChangesAsync();

                return entity;
            }
        }

        public Task<Guid> GetGoodByName(string name)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                var good = context.Goods.Where(x => x.Name == name).FirstOrDefault();

                if (good == null)
                    return Task.FromResult(Guid.Empty);
                return Task.FromResult(good.Id);
            }
        }

        public async Task<decimal> CalculateTotalQuantityOfGoods(Guid goodId, Guid storageId)
        {
            decimal totalQuantity = 0;

            using (DatabaseContext context = _factory.CreateDbContext())
            {
                totalQuantity = context.InventoryItemBases
                   .Where(x => x.GoodId == goodId && x.StorageId == storageId)
                   .Sum(x => x.Quantity);
            }

            return await Task.FromResult(totalQuantity);
        }


        public Guid? FindUnitByName(string unit)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                var _unit = context.MeasureUnits.FirstOrDefault(x => x.Name.ToLower() == unit.ToLower());
                return _unit?.Id;
            }
        }
    }
}
