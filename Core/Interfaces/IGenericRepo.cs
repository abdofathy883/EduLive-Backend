using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IGenericRepo<T> where T : class, IDeletable
    {
        Task<T?> GetByIdAsync(object id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        T Update(T entity);
        Task<bool> DeleteByIdAsync(object id);
        Task<bool> SaveAllAsync();
    }
}
