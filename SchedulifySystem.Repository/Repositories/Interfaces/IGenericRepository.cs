using Microsoft.EntityFrameworkCore.Query;
using SchedulifySystem.Repository.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {

        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
        Task<T> GetByIdAsync(int id);
        Task AddAsync(T entity);
        void Update(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = 1,
            int? pageSize = 20);
        Task<Pagination<T>> ToPaginationAsync(int pageIndex = 1, int pageSize = 20);

        Task<Pagination<T>> GetPaginationAsync(
          Expression<Func<T, bool>> filter = null,
          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
          string includeProperties = "",
          int? pageIndex = 1, // Optional parameter for pagination (page number)
          int? pageSize = 20);

        Task<Pagination<T>> ToPaginationIncludeAsync(
            int pageIndex = 1,
            int pageSize = 20,
            Func<IQueryable<T>,
            IIncludableQueryable<T, object>>? include = null,
            Expression<Func<T, bool>> filter = null,
          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null);

        Task<T?> GetByIdAsync(int id,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            Expression<Func<T, bool>> filter = null);
    }
}
