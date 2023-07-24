using AutoMapper;
using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.SearchObjects;
using ImporterPOS.Domain.Services.Generic;
using ImporterPOS.Domain.Services.Rules;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Storages
{
    public class StorageService : BaseCRUDService<Storage, StorageSearchObject>, IStorageService
    {
        public StorageService(DatabaseContextFactory factory) : base(factory)   {
        }

        public override ICollection<Storage> Get(StorageSearchObject search = null)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var entity = Context.Set<Storage>().AsQueryable();

                if (!string.IsNullOrWhiteSpace(search?.Name))
                {
                    entity = entity.Where(x => x.Name == search.Name);
                }

                return entity.ToList();
            }
        }

        public async Task<bool> Create(Models1.Storage entity)
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

        public async Task<ICollection<Models1.Storage>> Delete(Guid id)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Models1.Storage? entity = await context.Storages.FirstOrDefaultAsync(x => x.Id == id);
                if (entity != null)
                    context.Remove(entity);

                context.SaveChangesAsync();
                ICollection<Models1.Storage> entities = context.Storages.ToList();
                return entities;
            }
        }

        public async Task<Models1.Storage> Get(string id)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Models1.Storage? entity = await context.Storages.FirstOrDefaultAsync(x => x.Id.ToString() == id);
                return entity;
            }
        }

        public async Task<ICollection<Models1.Storage>> GetAll()
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                ICollection<Models1.Storage> entities = await context.Storages.ToListAsync();
                return entities;
            }
        }

        public async Task<Models1.Storage> Update(Guid id, Models1.Storage entity)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                entity.Id = id;
                context.Storages.Update(entity);
                await context.SaveChangesAsync();

                return entity;
            }
        }

        public Task<Guid> GetStorageByName(string name)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Storage storage = context.Storages.Where(x => x.Name == name).FirstOrDefault();
                if (storage == null)
                {
                    Storage s = new Storage
                    {
                        Id = Guid.NewGuid(),
                        Name = name,
                        Deleted = false
                    };
                    context.Storages.Add(s);
                    context.SaveChanges();
                    return Task.FromResult(s.Id);
                }
                return Task.FromResult(storage.Id);
            }
        }

       
    }
}
