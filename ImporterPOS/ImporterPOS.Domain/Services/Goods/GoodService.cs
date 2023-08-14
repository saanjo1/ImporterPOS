using AutoMapper;
using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.SearchObjects;
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
    public class GoodService : BaseCRUDService<Good, GoodSearchObject>, IGoodService
    {
        public GoodService(DatabaseContextFactory factory) : base(factory)
        {
        }

        public override ICollection<Good> Get(GoodSearchObject search = null)
        {
           using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var entity = Context.Set<Good>().AsQueryable();

                if (!string.IsNullOrWhiteSpace(search?.Name))
                {
                    entity = entity.Where(x => x.Name == search.Name);
                }

                return entity.ToList();
            }
        }

        public override Good Update(Guid id, Good request)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var set = Context.Set<Good>();

                Good? entity = set.Find(id);

                entity.Name = request.Name;
                entity.LatestPrice = request.LatestPrice;
                entity.UnitId = request.UnitId;
                entity.Volumen = request.Volumen;

                Context.SaveChanges();
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
