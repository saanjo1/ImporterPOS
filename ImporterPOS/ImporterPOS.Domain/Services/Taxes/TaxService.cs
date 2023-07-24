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

namespace ImporterPOS.Domain.Services.Taxes
{
    public class TaxService : BaseCRUDService<Taxis, TaxSearchObject>, ITaxService
    {
        public TaxService(DatabaseContextFactory factory) : base(factory)
        {
            
        }

        public override ICollection<Taxis> Get(TaxSearchObject search = null)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var entity = Context.Set<Taxis>().AsQueryable();

                if (!string.IsNullOrWhiteSpace(search?.Name))
                {
                    entity = entity.Where(x => x.Name == search.Name);
                }

                if (search?.Value != null)
                {
                    entity = entity.Where(x => x.Value == search.Value);
                }

                return entity.ToList();
            }
        }

        public void CreateTaxArticle(TaxArticle taxArticle)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                bool taxArticleExists = context.TaxArticles.Any(x => x.TaxId == taxArticle.TaxId && x.ArticleId == taxArticle.ArticleId);
                if (!taxArticleExists)
                {
                    context.TaxArticles.Add(taxArticle);
                    context.SaveChanges();
                }
            }
        }
    }
}
