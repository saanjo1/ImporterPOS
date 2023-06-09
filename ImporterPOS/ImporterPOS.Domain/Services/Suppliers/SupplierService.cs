using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.Services.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Suppliers
{
    public class SupplierService : ISupplierService
    {
        private readonly DatabaseContextFactory _factory;

        public SupplierService(DatabaseContextFactory factory)
        {
            _factory = factory;
        }

        public async Task<bool> Create(Supplier entity)
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

        public async Task<ICollection<Supplier>> Delete(Guid id)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Supplier? entity = await context.Suppliers.FirstOrDefaultAsync(x => x.Id == id);
                if (entity != null)
                    context.Remove(entity);

                context.SaveChangesAsync();
                ICollection<Supplier> entities = context.Suppliers.ToList();
                return entities;
            }
        }

        public async Task<Supplier> Get(string id)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Supplier entity = await context.Suppliers.FirstOrDefaultAsync(x => x.Id.ToString() == id);
                return entity;
            }
        }

        public async Task<ICollection<Supplier>> GetAll()
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                ICollection<Supplier> entities = await context.Suppliers.ToListAsync();
                return entities;
            }
        }

        public async Task<Supplier> Update(Guid id, Supplier entity)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                entity.Id = id;
                context.Suppliers.Update(entity);
                await context.SaveChangesAsync();

                return entity;
            }
        }

        public Task<Guid> GetSupplierByName(string name)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Supplier supplier = context.Suppliers.Where(x => x.Name == name).FirstOrDefault();
                if (supplier == null)
                {
                    Supplier s = new Supplier
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                    };
                    context.Suppliers.Add(s);
                    context.SaveChanges();
                    return Task.FromResult(s.Id);
                }
                return Task.FromResult(supplier.Id);
            }
        }

    }
}
