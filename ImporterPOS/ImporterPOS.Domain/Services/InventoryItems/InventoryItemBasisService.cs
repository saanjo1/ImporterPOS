using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.InventoryItems
{
    public class InventoryItemBasisService : IInventoryItemBasisService
    {
        private readonly DatabaseContextFactory _factory;

        public InventoryItemBasisService(DatabaseContextFactory factory)
        {
            _factory = factory;
        }

        public async Task<bool> Create(InventoryItemBasis entity)
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

        public async Task<ICollection<InventoryItemBasis>> Delete(Guid id)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                InventoryItemBasis? entity = await context.InventoryItemBases.FirstOrDefaultAsync(x => x.Id == id);
                if (entity != null)
                    context.Remove(entity);

                context.SaveChangesAsync();
                ICollection<InventoryItemBasis> entities = context.InventoryItemBases.ToList();
                return entities;
            }
        }

        public async Task<InventoryItemBasis> Get(string id)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                InventoryItemBasis? entity = await context.InventoryItemBases.FirstOrDefaultAsync(x => x.Id.ToString() == id);
                return entity;
            }
        }

        public async Task<ICollection<InventoryItemBasis>> GetAll()
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                ICollection<InventoryItemBasis> entities = await context.InventoryItemBases.ToListAsync();
                return entities;
            }
        }

        public async Task<InventoryItemBasis> Update(Guid id, InventoryItemBasis entity)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                entity.Id = id;
                context.InventoryItemBases.Update(entity);
                await context.SaveChangesAsync();

                return entity;
            }
        }
    }
}

