using Brainshaker.Domain.Repositories;

namespace Brainshaker.Infra.Repositories;

public abstract class AbstractRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    public Task AddAsync(TEntity entity)
    {
        return Task.CompletedTask;
        // throw new NotImplementedException();
    }

    public Task<TEntity?> GetByIdAsync(Guid id)
    {
        return Task.FromResult((TEntity)null);
        //return default;
        //throw new NotImplementedException();
    }
}