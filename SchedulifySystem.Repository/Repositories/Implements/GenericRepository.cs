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
using System.Linq.Expressions;

namespace SchedulifySystem.Repository.Repositories.Implements
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        internal DbSet<T> _dbSet;

        public GenericRepository(SchedulifyContext context)
        {
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }


        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<List<T>> GetAllAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task<IEnumerable<T>> GetAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Implementing pagination
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            return await query.ToListAsync();
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.CountAsync();
        }

        public async Task<Pagination<T>> GetPaginationAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            int? pageIndex = null,
            int? pageSize = null)
        {
            IQueryable<T> query = _dbSet;

            var itemCount = await CountAsync(filter);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            // Implementing pagination
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                // Ensure the pageIndex and pageSize are valid
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10; // Assuming a default pageSize of 10 if an invalid value is passed

                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            //return await query.ToListAsync();

            var result = new Pagination<T>()
            {
                PageSize = pageSize ?? 10,
                TotalItemCount = itemCount,
                PageIndex = pageIndex ?? 1,
                Items = await query.AsNoTracking().ToListAsync()
            };

            return result;
        }

        public async Task<Pagination<T>> ToPaginationAsync(int pageIndex = 1, int pageSize = 20)
        {
            // get total count of items in the db set
            var itemCount = await CountAsync();

            var result = new Pagination<T>()
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


        public async Task<Pagination<T>> ToPaginationIncludeAsync(
            int pageIndex = 1,
            int pageSize = 20,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null)
         {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            var itemCount = await query.CountAsync();

            var result = new Pagination<T>
            {
                PageSize = pageSize,
                TotalItemCount = itemCount,
                PageIndex = pageIndex
            };

            // Implement pagination
            int validPageIndex = pageIndex > 0 ? pageIndex - 1 : 0;
            query = query.Skip(validPageIndex * pageSize).Take(pageSize);

            var items = await query.AsNoTracking().ToListAsync();

            result.Items = items;

            return result;
        }

        public async Task<T?> GetByIdAsync(int id,
    Func<IQueryable<T>, IQueryable<T>>? include = null,
    Expression<Func<T, bool>> filter = null)
        {
            IQueryable<T> query = _dbSet;

            // Apply include if provided
            if (include != null)
            {
                query = include(query);
            }

            // Apply filter if provided
            if (filter != null)
            {
                query = query.Where(filter);
            }
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id);
        }
    }
}