using ImporterPOS.Domain.Models;
using ImporterPOS.Domain.Services.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImporterPOS.Domain.Services.Articles
{
    public class ArticleService : IArticleService
    {
        private readonly IRepository<Article> _articleRepository;

        public ArticleService(IRepository<Article> articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<IEnumerable<Article>> GetAllArticlesAsync()
        {
            return await _articleRepository.GetAllAsync();
        }

        public async Task<Article> GetArticleByIdAsync(string id)
        {
            return await _articleRepository.GetByIdAsync(id);
        }

        public async Task CreateArticleAsync(Article article)
        {
           await _articleRepository.AddAsync(article);
           
        }

        public async Task UpdateArticleAsync(Article article)
        {
            await _articleRepository.UpdateAsync(article);
        }

        public async Task DeleteArticleAsync(string id)
        {
            await _articleRepository.DeleteAsync(id);
        }
    }
}
