using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Articles
{
    public interface IArticleService : BaseInterface<Article>
    {
        Task<Guid> GetArticleIdByBarcode(string barcode);
        Task<int> GetCounter(Guid _subCategoryId);
        void SaveArticleGood(ArticleGood newArticleGood);
        Task<bool> CheckForNormative(Guid articleId);
        Task<Good> GetGoodFromArticleByName(string name);
        Task<decimal> GetTotalSellingPrice(InventoryDocument inventoryDocument);

        Task<decimal> GetTotalBasePrices(InventoryDocument inventoryDocument);

        Task<Article> GetPriceByGood(Guid? goodId);

        Guid? FindSubcategoryByName(string subcategory);
    }


}
