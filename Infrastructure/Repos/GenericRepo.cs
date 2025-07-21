using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
            var entity = await GetByIdAsync(id)
                ?? throw new ArgumentNullException(nameof(id), "Entity not found for deletion.");

            entity.IsDeleted = true;
            dbSet.Update(entity);
            return true;
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null)
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
                throw new ArgumentException("Entity with the given ID has been deleted.");

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
