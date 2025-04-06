using Brainshaker.Domain.Repositories;
using Brainshaker.Infra.Database;

namespace Brainshaker.Infra.Repositories;

public abstract class AbstractRepository<TEntity>(DatabaseContext context) : IRepository<TEntity> where TEntity : class
{
    public async Task AddAsync(TEntity entity)
    {
        await context.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await context.FindAsync<TEntity>(id);
    }
}