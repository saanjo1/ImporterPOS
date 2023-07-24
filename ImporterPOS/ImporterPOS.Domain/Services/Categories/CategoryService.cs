using AutoMapper;
using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.SearchObjects;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Categories
{
    public class CategoryService : BaseCRUDService<Category, CategorySearchObject>, ICategoryService
    {
        public CategoryService(DatabaseContextFactory factory) : base(factory)
        {

        }

        public override ICollection<Category> Get(CategorySearchObject search = null)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var entity = Context.Set<Category>().AsQueryable();

                if (search != null)
                {
                    if (!string.IsNullOrWhiteSpace(search?.Name))
                    {
                        entity = entity.Where(x => x.Name == search.Name);
                    }
                }

                return entity.ToList();
            }
        }
    }
}
