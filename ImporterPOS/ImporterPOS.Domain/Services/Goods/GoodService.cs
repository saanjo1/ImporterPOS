using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Generic;
using Microsoft.EntityFrameworkCore;
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
                Good good = context.Goods.Where(x => x.Name.Contains(name)).FirstOrDefault();
                if (good == null)
                    return Task.FromResult(Guid.Empty);
                return Task.FromResult(good.Id);
            }
        }

    }
}
