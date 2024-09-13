using Microsoft.EntityFrameworkCore.Query;
using SchedulifySystem.Repository.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAllAsync(Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);

        Task<T?> GetByIdAsync(int id);

        Task AddAsync(T model);

        void AddAttach(T model);
        void AddEntry(T model);
        void Update(T model);
        void Remove(T model);

        void UpdateRange(List<T> models);

        Task AddRangeAsync(List<T> models);

        // Add paging method to generic interface 
        Task<Pagination<T>> ToPaginationAsync(int pageIndex = 0, int pageSize = 10);
        Task<Pagination<T>> ToListPaginationAsync(IQueryable<T> query, int pageIndex = 0, int pageSize = 10);
        Task<Pagination<T>> ToPaginationIncludeAsync(int pageIndex = 0, int pageSize = 10, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
    }
}
