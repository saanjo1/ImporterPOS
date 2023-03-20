using ImporterPOS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Articles
{
    public interface IArticleService 
    {
        Task<IEnumerable<Article>> GetAllArticlesAsync();
        Task<Article> GetArticleByIdAsync(string id); 
        Task CreateArticleAsync(Article article);
        Task DeleteArticleAsync(string id);
        Task UpdateArticleAsync(Article article);
    }
}
