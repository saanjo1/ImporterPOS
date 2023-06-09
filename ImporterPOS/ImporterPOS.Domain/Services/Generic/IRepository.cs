using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Generic
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(string id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(string id);
        Task<Good> GetGoodByName(string name);
        Task<Article> GetArticleByBarcode(string barcode);
        Task<Storage> GetStorageByName(string storage);
        Task<Supplier> GetSupplierByName(string supplier);
        Task<int> GetNumberOfRecords();
        Task<Guid> GetSubCategoryId(string? category, string? storage);
        Task SaveArticleGood(ArticleGood newArticleGood);
        Task<bool> CheckForNormative(Guid articleId);
    }
}
