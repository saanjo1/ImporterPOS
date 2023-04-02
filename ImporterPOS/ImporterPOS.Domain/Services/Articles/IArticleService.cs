﻿using ImporterPOS.Domain.Models;
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
        Task<Guid> GetComparedByBarcode(string barcode);
        Task<int> GetCounter(Guid _subCategoryId);
        Task<Guid> ManageSubcategories(string? category, string? storage);
        void SaveArticleGood(ArticleGood newArticleGood);
        Task<bool> CheckForNormative(Guid articleId);
    }
}