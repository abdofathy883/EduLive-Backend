using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repos
{
    public class GenericRepo<T> : IGenericRepo<T> where T : class, IDeletable
    {
        private readonly E_LearningDbContext dbContext;
        private readonly DbSet<T> dbSet;

        public GenericRepo(E_LearningDbContext context)
        {
            dbContext = context;
            dbSet = dbContext.Set<T>();
        }
        public async Task<T> AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
            return entity;
        }

        public async Task<bool> DeleteByIdAsync(object id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
            {
                return false;
            }
            entity.IsDeleted = true;
            dbSet.Update(entity);
            return true;
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            return await dbContext.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await dbSet.Where(e => e.IsDeleted == false).AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(object id)
        {
            var entity = await dbSet.FindAsync(id);
            if (entity is not null && entity.IsDeleted)
            {
                entity = null;
            }
            return entity;
        }

        public async Task<bool> SaveAllAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }

        public T Update(T entity)
        {
            dbSet.Update(entity);
            return entity;
        }
    }
}
