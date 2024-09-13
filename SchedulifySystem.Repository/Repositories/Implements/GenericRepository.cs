using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore;
using SchedulifySystem.Repository.Commons;
using SchedulifySystem.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SchedulifySystem.Repository.DBContext;

namespace SchedulifySystem.Repository.Repositories.Implements
{
    public class GenericRepository<TModel> : IGenericRepository<TModel> where TModel : class
    {
        protected DbSet<TModel> _dbSet;

        public GenericRepository(SchedulifyContext context)
        {
            _dbSet = context.Set<TModel>();
        }

        public virtual async Task AddAsync(TModel model)
        {
            await _dbSet.AddAsync(model);
        }

        public virtual void AddAttach(TModel model)
        {
            _dbSet.Attach(model).State = EntityState.Added;
        }

        public virtual void AddEntry(TModel model)
        {
            _dbSet.Entry(model).State = EntityState.Added;
        }

        public virtual async Task AddRangeAsync(List<TModel> models)
        {
            await _dbSet.AddRangeAsync(models);
        }

        public virtual async Task<List<TModel>> GetAllAsync() => await _dbSet.ToListAsync();

        public virtual async Task<List<TModel>> GetAllAsync(Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>? include = null)
        {
            IQueryable<TModel> query = _dbSet;
            if (include != null)
            {
                query = include(query);
            }
            return await query.ToListAsync();
        }

        public virtual async Task<TModel?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public void Update(TModel? model)
        {
            _dbSet.Update(model);
        }

        public void UpdateRange(List<TModel> models)
        {
            _dbSet.UpdateRange(models);
        }

        public void Remove(TModel model)
        {
            _dbSet.Remove(model);
        }

        public async Task<Pagination<TModel>> ToPaginationAsync(int pageIndex = 0, int pageSize = 10)
        {
            var itemCount = await _dbSet.CountAsync();

            var result = new Pagination<TModel>()
            {
                PageSize = pageSize,
                TotalItemCount = itemCount,
                PageIndex = pageIndex,
            };

            var items = await _dbSet.Skip(result.PageIndex * result.PageSize)
                .Take(result.PageSize)
                .AsNoTracking()
                .ToListAsync();

            result.Items = items;

            return result;
        }

        public async Task<Pagination<TModel>> ToListPaginationAsync(IQueryable<TModel> query, int pageIndex = 0, int pageSize = 10)
        {
            var itemCount = await query.CountAsync();
            var result = new Pagination<TModel>()
            {
                PageSize = pageSize,
                TotalItemCount = itemCount,
                PageIndex = pageIndex,
            };
            var items = await query.Skip(result.PageIndex * result.PageSize)
                                   .Take(result.PageSize)
                                   .AsNoTracking()
                                   .ToListAsync();

            result.Items = items;

            return result;
        }

        public async Task<Pagination<TModel>> ToPaginationIncludeAsync(int pageIndex = 0, int pageSize = 10, Func<IQueryable<TModel>, IIncludableQueryable<TModel, object>>? include = null)
        {
            IQueryable<TModel> query = _dbSet;

            if (include != null)
            {
                query = include(query);
            }

            var itemCount = await query.CountAsync();

            var result = new Pagination<TModel>
            {
                PageSize = pageSize,
                TotalItemCount = itemCount,
                PageIndex = pageIndex
            };

            var items = await query
                .Skip(result.PageIndex * result.PageSize)
                .Take(result.PageSize)
                .AsNoTracking()
                .ToListAsync();

            result.Items = items;

            return result;
        }

    }
}
