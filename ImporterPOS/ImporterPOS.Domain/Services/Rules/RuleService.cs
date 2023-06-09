using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Rules
{
    public class RuleService : IRuleService
    {
        private readonly DatabaseContextFactory _factory;

        public RuleService(DatabaseContextFactory factory)
        {
            _factory = factory;
        }

        public async Task<bool> Create(Rule entity)
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

        public Task<bool> CreateRuleItem(RuleItem _ruleItem)
        {
            using(DatabaseContext context = _factory.CreateDbContext())
            {
                try
                {
                    context.Add(_ruleItem);
                    context.SaveChanges();
                    return Task.FromResult(true);
                }
                catch (Exception)
                {
                    return Task.FromResult(false);
                }
            }
        }

        public async Task<ICollection<Rule>> Delete(Guid id)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Rule? entity = await context.Rules.FirstOrDefaultAsync(x => x.Id == id);
                if (entity != null)
                    context.Remove(entity);

                context.SaveChangesAsync();
                ICollection<Rule> entities = context.Rules.ToList();
                return entities;
            }
        }

        public async Task<Rule> Get(string id)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Rule? entity = await context.Rules.FirstOrDefaultAsync(x => x.Id.ToString() == id);
                return entity;
            }
        }

        public async Task<ICollection<Rule>> GetAll()
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                var entities = await context.Rules.ToListAsync();
                return entities;
            }
        }

        public Task<Rule> GetRuleByName(string name)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Rule _discount = context.Rules.FirstOrDefault(x => x.Name == name);
                return Task.FromResult(_discount);
            }
        }

        public async Task<Rule> Update(Guid id, Rule entity)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                entity.Id = id;
                context.Rules.Update(entity);
                await context.SaveChangesAsync();

                return entity;
            }
        }
    }
}
