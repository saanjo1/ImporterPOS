using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using ImporterPOS.Domain.Models1;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using ImporterPOS.Domain.SearchObjects;
using ImporterPOS.Domain.Services.Generic;
using AutoMapper;

namespace ImporterPOS.Domain.Services.Articles
{
    public class ArticleService : BaseCRUDService<Article, ArticleSearchObject>, IArticleService
    {
        public ArticleService(DatabaseContextFactory factory) : base(factory)
        {

        }
        public override ICollection<Article> Get(ArticleSearchObject search = null)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var entity = Context.Set<Article>().AsQueryable();

                if (search != null)
                {
                    if (!string.IsNullOrWhiteSpace(search?.Name))
                    {
                        entity = entity.Where(x => x.Name == search.Name);
                    }

                    if (!string.IsNullOrWhiteSpace(search?.BarCode))
                    {
                        entity = entity.Where(x => x.Name == search.BarCode);
                    }

                    var list = entity.ToList();
                }

                return entity.ToList();
            }
        }

        public override Article Update(Guid id, Article request)
        {
            using (DatabaseContext Context = _factory.CreateDbContext())
            {
                var set = Context.Set<Article>();

                Article? entity = set.Find(id);

                entity.Name = request.Name;
                entity.Price = request.Price;
                entity.BarCode = request.BarCode;
                entity.SubCategoryId = request.SubCategoryId;
                entity.Order = request.Order;
                entity.Deleted = request.Deleted;
                entity.ReturnFee = request.ReturnFee;
                entity.Code = request.Code;
                entity.Tag = request.Tag;

                return entity;
            }
        }


        public Task<int> GetCounter(Guid _subCategoryId)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                if (_subCategoryId != Guid.Empty)
                {
                    return Task.FromResult(context.Articles.Where(x => x.SubCategoryId == _subCategoryId).Count() + 1);
                }
                return Task.FromResult(context.Articles.Count() + 1);
            }
        }


        public void SaveArticleGood(ArticleGood newArticleGood)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                context.Add(newArticleGood);
                context.SaveChanges();
            }
        }

        public Task<bool> CheckForNormative(Guid _articleId)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                var x = context.ArticleGoods.Where(x => x.ArticleId == _articleId).FirstOrDefault();
                if (x != null)
                    return Task.FromResult(true);
                return Task.FromResult(false);
            }
        }




    }
}
