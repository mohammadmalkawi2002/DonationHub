using DonationHub.Application.Common.Interfaces;
using DonationHub.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonationHub.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity>(AppDbContext dbContext) : IGenericRepository<TEntity>
        where TEntity : Entity
    {
        private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();
        

        public async Task AddAsync(TEntity entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
        }

        public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
           return await _dbSet.FirstOrDefaultAsync(x=>x.Id == id, ct);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }
    }
}
