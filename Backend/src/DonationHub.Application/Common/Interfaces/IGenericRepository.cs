using DonationHub.Domain.Common;
using System.Linq.Expressions;

namespace DonationHub.Application.Common.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : Entity
    {
        Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);

        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);

        /// <summary>
        /// for Check  exisiting rather than get the full entity 
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(
      Expression<Func<TEntity, bool>> predicate,
      CancellationToken ct = default,
      bool ignoreQueryFilters = false);
        Task AddAsync(TEntity entity, CancellationToken ct = default);
        void Update(TEntity entity);

        void Remove(TEntity entity);
    }
}
