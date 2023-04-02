using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.InventoryDocuments
{
    public class InventoryDocumentsService : IInventoryDocumentsService
    {
        private readonly DatabaseContextFactory _factory;

        public InventoryDocumentsService(DatabaseContextFactory factory)
        {
            _factory = factory;
        }

        public async Task<bool> Create(InventoryDocument entity)
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

        public async Task<ICollection<InventoryDocument>> Delete(Guid id)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                InventoryDocument? entity = await context.InventoryDocuments.FirstOrDefaultAsync(x => x.Id == id);
                if (entity != null)
                    context.Remove(entity);

                context.SaveChangesAsync();
                ICollection<InventoryDocument> entities = context.InventoryDocuments.ToList();
                return entities;
            }
        }

        public async Task<InventoryDocument> Get(string id)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                InventoryDocument? entity = await context.InventoryDocuments.FirstOrDefaultAsync(x => x.Id.ToString() == id);
                return entity;
            }
        }

        public async Task<ICollection<InventoryDocument>> GetAll()
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                ICollection<InventoryDocument> entities = await context.InventoryDocuments.ToListAsync();
                return entities;
            }
        }

        public async Task<InventoryDocument> Update(Guid id, InventoryDocument entity)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                entity.Id = id;
                context.InventoryDocuments.Update(entity);
                await context.SaveChangesAsync();

                return entity;
            }
        }

        public Task<int> GetInventoryOrderNumber()
        {
            using(DatabaseContext context = _factory.CreateDbContext())
            {
                return Task.FromResult(context.InventoryDocuments.Count());
            }
        }

    }
}
