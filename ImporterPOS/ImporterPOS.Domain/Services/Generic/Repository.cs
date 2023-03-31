using ImporterPOS.Domain.EF;
using ImporterPOS.Domain.Models;
using Microsoft.EntityFrameworkCore;
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

        public async Task<TEntity> GetByNameAsync(string name, int type)
        {
            // Get the entity type for the current repository
            Type entityType = typeof(TEntity);

            // Get the DbSet for the specified entity type using reflection
            PropertyInfo? dbSetProperty = _dbContext.GetType().GetProperty(entityType.Name +"s");
            DbSet<TEntity>? dbSet = dbSetProperty.GetValue(_dbContext) as DbSet<TEntity>;

            // Get the Name property for the entity type
            PropertyInfo? nameProperty = entityType.GetProperty("Name");

            IEnumerable<TEntity> query = dbSet.AsEnumerable().Where(e => nameProperty.GetValue(e).ToString() == name);

            if (type == 1)
            {
                query = dbSet.AsEnumerable().Where(e => nameProperty.GetValue(e).ToString().Contains(name));
            }
            TEntity? result = query.FirstOrDefault();
            if (result == null)
                return null;
            return result;
        }

        public async Task<int> GetNumberOfRecords()
        {
            return await _dbSet.CountAsync();
        }
    }

}
