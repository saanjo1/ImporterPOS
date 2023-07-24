using AutoMapper;
using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.SearchObjects;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Units
{
    public class UnitService : BaseCRUDService<MeasureUnit, MeasureUnitSearchObject>, IUnitService
    {
        public UnitService(DatabaseContextFactory factory) : base(factory)
        {
            
        }

        public override ICollection<MeasureUnit> Get(MeasureUnitSearchObject search)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var entity = Context.Set<MeasureUnit>().AsQueryable();
                
                if(!string.IsNullOrWhiteSpace(search?.Name)) 
                { 
                    entity = entity.Where(x=>x.Name ==  search.Name);
                }

                if(!string.IsNullOrWhiteSpace(search?.Id))
                {
                    entity = entity.Where(x=>x.Id.ToString() == search.Id);
                }

                return entity.ToList();
            }
        }
    }
}
