using EFCore.BulkExtensions;
using ELM.Common.DTO;
using ELM.Common.Entities.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ELM.Customers.Database.DAL
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
      where TEntity : BaseEntity
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        private readonly string entityName;
        public GenericRepository(DbContext context)
        {
            entityName = typeof(TEntity).ToString();
            this._context = context;
            this._dbSet = context.Set<TEntity>();
        }

        public void Dettach(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }
        public virtual async Task<IEnumerable<TEntity>> ReadAll(
            string[] includeProperties = null,
            GridOptions<TEntity> gridOptions = null,
            string orderBy = null, bool orderByDescending = false,
            int recordsIndex = -1, int pageIndex = -1, int pageSize = -1, bool asNoTracking = false)
        {
            IQueryable<TEntity> query = _dbSet;
            bool fetchFromDb = true;

            // search & filteration
            if (gridOptions != null && gridOptions.Filters != null & gridOptions.Filters.Count > 0)
            {
                foreach (var filter in gridOptions.Filters)
                {
                    query = query.Where(filter);
                }
            }

            //include
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            //sorting
            if (gridOptions != null && gridOptions.Order != null & gridOptions.Order.Count > 0)
            {
                IOrderedQueryable<TEntity> orderedQuery = null;
                OrderBy<TEntity> orderFirstItem = gridOptions.Order[0];
                if (orderFirstItem.Desc)
                {
                    //orderedQuery = query.OrderByDescending(e => EF.Property<object>(e, orderFirstItem.Column));
                    orderedQuery = query.OrderByDescending(orderFirstItem.Property);
                }
                else
                {
                    //orderedQuery = query.OrderBy(e => EF.Property<object>(e, orderFirstItem.Column));
                    orderedQuery = query.OrderBy(orderFirstItem.Property);
                }

                for (int i = 1; i < gridOptions.Order.Count; i++)
                {
                    OrderBy<TEntity> orderItem = gridOptions.Order[i];
                    if (orderItem.Desc)
                    {
                        //orderedQuery = orderedQuery.ThenByDescending(e => EF.Property<object>(e, orderItem.Column));
                        orderedQuery = orderedQuery.ThenByDescending(orderItem.Property);
                    }
                    else
                    {
                        //orderedQuery = orderedQuery.ThenBy(e => EF.Property<object>(e, orderItem.Column));
                        orderedQuery = orderedQuery.ThenBy(orderItem.Property);
                    }
                }

                query = orderedQuery;
            }

            // pagination
            if (recordsIndex != -1 && pageSize != -1)
            {
                query = query.Skip(recordsIndex).Take(pageSize);
            }
            else if (pageIndex != -1 && pageSize != -1)
            {
                query = query.Skip(pageIndex * pageSize).Take(pageSize);
            }
            //check it
            if (asNoTracking)
            {
                return query.AsNoTracking().ToList();
            }
            var result = query.ToList();
            return result;
        }

        public virtual async Task<int> Count(GridOptions<TEntity> filters = null)
        {
            IQueryable<TEntity> query = _dbSet;

            // search & filteration
            if (filters != null)
            {
                foreach (var filter in filters.Filters)
                {
                    query = query.Where(filter);
                }
            }

            return await query.CountAsync();
        }
        public async Task<bool> Any(GridOptions<TEntity> filters = null)
        {
            IQueryable<TEntity> query = _dbSet;

            // search & filteration
            if (filters != null)
            {
                foreach (var filter in filters.Filters)
                {
                    query = query.Where(filter);
                }
            }

            return await query.AnyAsync();
        }
        public async Task<TEntity> ReadOneById(int id)
        {
            GridOptions<TEntity> filters = new GridOptions<TEntity>();
            filters.AddFilter(d => d.Id == id);
            var all = await ReadAll(gridOptions: filters);
            var record = all.FirstOrDefault();
            return record;
        }
        public async Task<TEntity> ReadOneSingle(int id, Expression<Func<TEntity, bool>> condition,
            string[] includeProperties = null, bool asNoTracking = false)
        {
            IQueryable<TEntity> query = _dbSet;

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }
            return await query.SingleOrDefaultAsync(condition);
        }
        public async Task<bool> Exists(int id)
        {
            return (await _dbSet.FindAsync(id) != null);
        }
        public void Create(TEntity entity)
        {
            try
            {
                _dbSet.Add(entity);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public void Update(TEntity entity)
        {
            Dettach(entity);
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                try
                {
                    _dbSet.Attach(entity);
                }
                catch (Exception ex)
                {
                    //Ignore this error, DI already has the same DB context injected that's why the entity can not be tracked twice.
                }
            }
            _context.Entry(entity).State = EntityState.Modified;
        }
        private void HandleDelete(TEntity entity, bool hardDelete = false)
        {
            if (entity == null) return;
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            if (hardDelete)
            {
                //_context.Entry(entity).State = EntityState.Deleted;
                _context.Remove(entity);
            }
            else
            {
                _context.Entry(entity).State = EntityState.Modified;
            }
        }
        public void Delete(TEntity entity, bool hardDelete = false)
        {
            HandleDelete(entity, hardDelete);
        }
        public void Delete(string id)
        {
            TEntity entity = _dbSet.Find(id);
            HandleDelete(entity, false);
        }
        public void LoadCollection(TEntity entity, string property)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _dbSet.Attach(entity);
            }

            _context.Entry(entity)
                .Collection(property)
                .Load();
        }
        public async Task<int> Save()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task Create(List<TEntity> entites)
        {
            try
            {
                foreach(var entity in entites)
                {
                    (entity as BaseEntity).CreatedAt = DateTime.Now; ;
                    (entity as BaseEntity).CreatedByUserId = "SystemAdmin";
                }
                await _context.BulkInsertAsync(entites);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
