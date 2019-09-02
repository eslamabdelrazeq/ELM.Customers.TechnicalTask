using ELM.Common.DTO;
using ELM.Common.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ELM.Customers.Database.DAL
{

    public interface IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        Task<IEnumerable<TEntity>> ReadAll(
            string[] includeProperties = null,
            GridOptions<TEntity> gridOptions = null,
            string orderBy = null, bool orderByDescending = false,
            int recordsIndex = -1, int pageIndex = -1, int pageSize = -1, bool asNoTracking = false);

        Task<TEntity> ReadOneById(int id);
        Task<TEntity> ReadOneSingle(int id, Expression<Func<TEntity, bool>> condition, string[] includeProperties = null, bool asNoTracking = false);
        Task<bool> Exists(int id);
        Task<int> Count(GridOptions<TEntity> filters = null);
        Task<bool> Any(GridOptions<TEntity> filters = null);
        void LoadCollection(TEntity entity, string property);
        void Dettach(TEntity entity);
        void Create(TEntity entity);
        Task Create(List<TEntity> entites);
        void Update(TEntity entity);
        void Delete(TEntity entity, bool hardDelete = false);
        void Delete(string id);
        Task<int> Save();
    }

}
