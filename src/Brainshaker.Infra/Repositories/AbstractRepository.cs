using Brainshaker.Domain.Entities;
using Brainshaker.Domain.Repositories;
using Brainshaker.Infra.Database;
using Microsoft.EntityFrameworkCore;

namespace Brainshaker.Infra.Repositories;

public abstract class AbstractRepository<TEntity>(DatabaseContext context) : IRepository<TEntity> where TEntity : AuditableEntities
{
    public async Task AddAsync(TEntity entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        await context.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<TEntity?> FindByIdAsync(int id)
    {
        return await context.Set<TEntity>().FirstOrDefaultAsync(x=>x.Id == id);
    }
}