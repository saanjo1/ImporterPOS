using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensions.Msal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Generic
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DatabaseContext _dbContext;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(DatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entity)
        {
            _dbSet.Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var entityToDelete = await _dbSet.FindAsync(id);
            _dbSet.Remove(entityToDelete);
            await _dbContext.SaveChangesAsync();
        }

        public Task<Article> GetArticleByBarcode(string barcode)
        {
           using (var context = new DatabaseContext())
            {
                Article article = _dbContext.Articles.Where(x => x.BarCode == barcode).FirstOrDefault();
                if (article != null)
                {
                    return Task.FromResult(article);
                }
                return null;
            }
        }

        public Task<Models1.Storage> GetStorageByName(string storage)
        {
            using (var context = new DatabaseContext())
            {
                Models1.Storage _storage = _dbContext.Storages.Where(x => x.Name == storage).FirstOrDefault();
                if (_storage != null)
                    return Task.FromResult(_storage);
                return null;
            }

        }
        
        public Task<Supplier> GetSupplierByName(string supplier)
        {
            using(var context = new DatabaseContext())
            {
                Supplier _supplier = context.Suppliers.Where(x => x.Name == supplier).FirstOrDefault();
                if (_supplier != null)
                    return Task.FromResult(_supplier);
                return null;
            }

        }

        public Task<int> GetNumberOfRecords()
        {
            return Task.FromResult(_dbSet.Count());
        }

        public Task<Guid> GetSubCategoryId(string? category, string? storage)
        {
            using (var context = new DatabaseContext())
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
                        Order = _dbContext.SubCategories.Count() + 1,
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

        private Task<Guid> ManageCategories(string? category, string? storage)
        {
            using (var context = new DatabaseContext())
            {
                Category _category = context.Categories.Where(x => x.Name == category).FirstOrDefault();

                if (_category == null)
                {
                    Category newCategory = new Category()
                    {
                        Id = Guid.NewGuid(),
                        Name = category,
                        Deleted = false,
                        Order = _dbContext.Categories.Count() + 1,
                        StorageId = new Guid(storage)
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

        public async Task SaveArticleGood(ArticleGood newArticleGood)
        {
             using (var context = new DatabaseContext())
            {
                context.Add(newArticleGood);
                await context.SaveChangesAsync();
            }
        }

        public Task<bool> CheckForNormative(Guid articleId)
        {
           using (var context = new DatabaseContext())
            {
                var x = context.ArticleGoods.Where(x => x.ArticleId == articleId).FirstOrDefault();
                if (x != null)
                    return Task.FromResult(true);
                return Task.FromResult(false);
            }
        }

        public Task<Good?> GetGoodByName(string name)
        {
            using (var context = new DatabaseContext())
            {
                Good? good = context.Goods.FirstOrDefault(x => x.Name == name);
                return Task.FromResult(good);
            }
        }
    }

}
