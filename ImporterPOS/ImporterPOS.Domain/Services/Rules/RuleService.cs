using AutoMapper;
using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.SearchObjects;
using ImporterPOS.Domain.Services.Generic;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Rules
{
    public class RuleService : BaseCRUDService<Rule, RuleSearchObject>, IRuleService
    {
        public RuleService(DatabaseContextFactory factory) : base(factory)
        {
        }

        public override ICollection<Rule> Get(RuleSearchObject search = null)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var entity = Context.Set<Rule>().AsQueryable();

                if(!string.IsNullOrWhiteSpace(search?.Name))
                {
                    entity = entity.Where(x=>x.Name == search.Name);
                }

                return entity.ToList();
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


        public Task<Rule> GetRuleByName(string name)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Rule _discount = context.Rules.FirstOrDefault(x => x.Name == name);
                return Task.FromResult(_discount);
            }
        }

    }
}
