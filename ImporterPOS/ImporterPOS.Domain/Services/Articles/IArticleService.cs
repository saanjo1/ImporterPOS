using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Models1;
using ImporterPOS.Domain.SearchObjects;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Articles
{
    public interface IArticleService : ICRUDService<Article, ArticleSearchObject>
    {
        Task<int> GetCounter(Guid _subCategoryId);
        void SaveArticleGood(ArticleGood newArticleGood);
        Task<bool> CheckForNormative(Guid articleId);
    }


}
