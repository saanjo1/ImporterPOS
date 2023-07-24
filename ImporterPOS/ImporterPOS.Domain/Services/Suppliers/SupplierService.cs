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
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Suppliers
{
    public class SupplierService : BaseCRUDService<Supplier, SupplierSearchObject>, ISupplierService
    {
        private readonly DatabaseContextFactory _factory;

        public SupplierService(DatabaseContextFactory factory) : base(factory)
        {
            _factory = factory;
        }

        public override ICollection<Supplier> Get(SupplierSearchObject search = null)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                IQueryable<Supplier> entity = Context.Set<Supplier>().AsQueryable();

                if (search != null)
                {
                    if (!string.IsNullOrWhiteSpace(search?.Name))
                    {
                        entity = entity.Where(x => x.Name == search.Name);
                    }
                    if (!string.IsNullOrWhiteSpace(search?.Id))
                    {
                        Guid? id = Guid.Parse(search?.Id);
                        entity = entity.Where(x => x.Id == id);
                    }
                }

                return entity.ToList();
            }
        }



    }
}
