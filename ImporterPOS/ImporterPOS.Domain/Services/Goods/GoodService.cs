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

        public Task<Guid> GetGoodByName(string name, bool stockCorrection = false)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Good good = new Good();
                if (stockCorrection)
                {
                    good = context.Goods.Where(x => x.Name == name).FirstOrDefault();
                }
                else
                {
                    good = context.Goods.Where(x => x.Name.Contains(name)).FirstOrDefault();
                }
                if (good == null)
                    return Task.FromResult(Guid.Empty);
                return Task.FromResult(good.Id);
            }
        }

        public async Task<decimal> SumQuantityOfGoodsById(Guid goodId, Guid storageId)
        {
            decimal sumQuantities = 0;

            using (DatabaseContext context = _factory.CreateDbContext())
            {
                sumQuantities = context.InventoryItemBases.
                   Where(x => x.GoodId == goodId && x.StorageId == storageId).Sum(x => x.Quantity);

            }

            return await Task.FromResult(sumQuantities);
        }

        public async Task SetMainStockToZero()
        {
            decimal quantity = 0;
            DateTime _date = new DateTime(2023, 04, 03, 06, 30, 00);

            using (DatabaseContext context = _factory.CreateDbContext())
            {
                InventoryDocument inventoryDocument = new InventoryDocument()
                {
                    Id = Guid.NewGuid(),
                    Created = _date,
                    Order = context.InventoryDocuments.Count() + 1,
                    IsActivated = true,
                    IsDeleted = false,
                    StorageId = new Guid("5C6BACE6-1640-4606-969D-000B25F422C6"),
                    Type = 2
                };

                context.InventoryDocuments.Add(inventoryDocument);
                foreach (Good goodEntity in context.Goods)
                {
                    if(goodEntity.Id == new Guid("2BA8AD29-7AFB-43FC-BC6F-8B2DBA622693"))
                    {
                        var x = 0;
                    }

                    bool goodExists = await context.InventoryItemBases
                        .AnyAsync(g => g.GoodId == goodEntity.Id && g.Created < _date);

                    if (goodExists)
                    {
                        var temp = context.InventoryItemBases
                            .Where(x => x.GoodId == goodEntity.Id && x.Created < _date)
                            .Sum(x => x.Quantity);

                        if (temp != 0)
                            quantity = -temp;
                        else
                            quantity = 0;
                    }

                   
                    InventoryItemBasis inventoryItemBasis = new InventoryItemBasis
                    {
                        Id = Guid.NewGuid(),
                        StorageId = new Guid("5C6BACE6-1640-4606-969D-000B25F422C6"),
                        Created = _date,
                        Quantity = quantity,
                        CurrentQuantity = quantity,
                        Tax = 0,
                        Discriminator = "InventoryDocumentItem",
                        InventoryDocumentId = inventoryDocument.Id,
                        GoodId = goodEntity.Id,
                        Price = goodEntity.LatestPrice,
                        Total = quantity * goodEntity.LatestPrice,
                        IsDeleted = false,
                        Refuse = 0
                    };

                    context.InventoryItemBases.Add(inventoryItemBasis);
                    quantity = 0;
                }


                await context.SaveChangesAsync();
            }
        }
    }
}
