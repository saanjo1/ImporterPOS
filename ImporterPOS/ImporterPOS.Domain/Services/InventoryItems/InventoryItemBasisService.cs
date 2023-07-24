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

namespace ImporterPOS.Domain.Services.InventoryItems
{
    public class InventoryItemBasisService : BaseCRUDService<InventoryItemBasis, InventoryItemBasesSearchObject>, IInventoryItemBasisService
    {
        public InventoryItemBasisService(DatabaseContextFactory factory) : base(factory)
        {
            
        }

        public override ICollection<InventoryItemBasis> Get(InventoryItemBasesSearchObject search = null)
        {
            using(DatabaseContext Context = _factory.CreateDbContext())
            {
                var entity = Context.Set<InventoryItemBasis>().AsQueryable();

                if (!string.IsNullOrWhiteSpace(search?.InventoryId))
                {
                    entity = entity.Where(x => x.Id.ToString() == search.InventoryId);
                }

                var list = entity.ToList();

                return entity.ToList();
            }
        }

        public Task<ICollection<InventoryItemBasis>> GetItemsByInventoryId(string inventoryId)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                ICollection<InventoryItemBasis> entities = context.InventoryItemBases.Where(x=>x.InventoryDocumentId.ToString() == inventoryId).ToList();
                return Task.FromResult(entities);
            }
        }


    }
}

