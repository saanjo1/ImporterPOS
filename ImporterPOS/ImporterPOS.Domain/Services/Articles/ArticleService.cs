using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client.Extensions.Msal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Articles
{
    public class ArticleService : IArticleService
    {
        private readonly DatabaseContextFactory _factory;

        public ArticleService(DatabaseContextFactory factory)
        {
            _factory = factory;
        }


        public async Task<ICollection<Article>> GetAll()
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                ICollection<Article> entities = await context.Articles.ToListAsync();
                return await Task.FromResult(entities);
            }
        }

        public async Task<Article> Get(string id)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Article? entity = await context.Articles.FirstOrDefaultAsync(x => x.Id.ToString() == id);
                return entity;
            }
        }

        public async Task<bool> Create(Article entity)
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

        public async Task<Article> Update(Guid id, Article entity)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                entity.Id = id;
                context.Articles.Update(entity);
                await context.SaveChangesAsync();

                return entity;
            }
        }

        public async Task<ICollection<Article>> Delete(Guid id)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Article? entity = await context.Articles.FirstOrDefaultAsync(x => x.Id == id);
                entity.Deleted = true;

                context.SaveChangesAsync();
                ICollection<Article> entities = context.Articles.ToList();
                return entities;
            }
        }

        public Task<Guid> GetComparedByBarcode(string barcode)
        {
           using (DatabaseContext context = _factory.CreateDbContext())
            {
                Article x = context.Articles.Where(x=>x.BarCode == barcode).FirstOrDefault();
                if (x != null)
                    return Task.FromResult(x.Id);
                return Task.FromResult(Guid.Empty);
            }
        }

        public Task<int> GetCounter(Guid _subCategoryId)
        {
            using (DatabaseContext context = _factory.CreateDbContext()) 
            {
                if(_subCategoryId != Guid.Empty)
                {
                    return Task.FromResult(context.Articles.Where(x=>x.SubCategoryId == _subCategoryId).Count() + 1);
                }
                return Task.FromResult(context.Articles.Count() + 1);
            }
        }

        public Task<Guid> ManageSubcategories(string? category, string? storage)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                SubCategory? _subCategory = context.SubCategories.Where(x => x.Name == category).FirstOrDefault();
                Guid _storageId = context.Storages.Where(x => x.Name.Contains("Glavno")).FirstOrDefault().Id;
                Guid _categoryId = this.ManageCategories(category, _storageId.ToString()).Result;

                if (_subCategory == null)
                {
                    SubCategory subCategory = new SubCategory()
                    {
                        Id = Guid.NewGuid(),
                        Name = category,
                        Deleted = false,
                        StorageId = _storageId,
                        CategoryId = _categoryId,
                        Order = context.SubCategories.Count() + 1,
                    };

                    var id = subCategory.Id;
                    context.SubCategories.Add(subCategory);
                    context.SaveChanges();
                    return Task.FromResult(id);
                }
                else
                {
                    return Task.FromResult(_subCategory.Id);
                }
            }
        }

        public Task<Guid> ManageCategories(string category, string storageId)
        {
            using(DatabaseContext context = _factory.CreateDbContext())
            {
                Category _category = context.Categories.Where(x => x.Name == category).FirstOrDefault();

                if (_category == null)
                {
                    Category newCategory = new Category()
                    {
                        Id = Guid.NewGuid(),
                        Name = category,
                        Deleted = false,
                        Order = context.Categories.Count() + 1,
                        StorageId = new Guid(storageId)
                    };
                    var id = newCategory.Id;
                    context.Categories.Add(newCategory);
                    context.SaveChanges();
                    return Task.FromResult(id);
                }
                else
                {
                    return Task.FromResult(_category.Id);
                }
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
          public Task<Good> GetGoodFromArticleByName(string name)
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                Article _article = context.Articles.Where(x=>x.Tag.Contains(name)).FirstOrDefault();
                if (_article != null)
                {
                    Good _good = context.Goods.Where(x => x.Name.Contains(_article.BarCode)).FirstOrDefault();
                    if (_good == null)
                    {
                        Good newGood = new Good
                        {
                            Id = Guid.NewGuid(),
                            Name = _article.Name,
                            UnitId = new Guid("5C6BACE6-1640-4606-969D-000B25F422C6"),
                            LatestPrice = 0,
                            Volumen = 1,
                            Refuse = 0
                        };
                        context.Goods.Add(newGood);

                        ArticleGood articleGood = new ArticleGood()
                        {
                            Id = Guid.NewGuid(),
                            ArticleId = _article.Id,
                            GoodId = newGood.Id,
                            ValidFrom = DateTime.Now,
                            ValidUntil = DateTime.Now.AddYears(30)
                        };
                        context.ArticleGoods.Add(articleGood);

                        context.SaveChanges();

                        return Task.FromResult(newGood);
                    }
                    else
                    {
                        return Task.FromResult(_good);
                    }

                }
                else
                {
                    Good defaultGood = new Good(); // create a default instance of Good
                    return Task.FromResult(defaultGood); // return the default instance
                }
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

        public Task<string> ConnectArticlesToGoods()
        {
            using (DatabaseContext context = _factory.CreateDbContext())
            {
                try
                {
                    int articlesCount = context.Articles.Count();
                    int goodsCount = context.Goods.Count();

                    int successCounter = 0;
                    if (goodsCount > 0 && articlesCount > 0)
                    {
                        foreach (var good in context.Goods)
                        {
                            foreach (var article in context.Articles)
                            {
                                if (good.Name.Contains(article.BarCode))
                                {
                                    ArticleGood newArticleGood = new ArticleGood
                                    {
                                        ArticleId = article.Id,
                                        GoodId = good.Id,
                                        ValidFrom = DateTime.Now,
                                        ValidUntil = DateTime.Now.AddYears(50),
                                        Quantity = 1,
                                        Id = Guid.NewGuid()
                                    };

                                    context.Add(newArticleGood);
                                    successCounter++;
                                }
                            }
                        }
                    }

                    string result = successCounter + "/" + articlesCount + " articles connected.";

                    return Task.FromResult(result);
                }
                catch
                {
                    return Task.FromResult("An error occurred while connecting articles to goods.");
                }

            }
        }
    }
}
