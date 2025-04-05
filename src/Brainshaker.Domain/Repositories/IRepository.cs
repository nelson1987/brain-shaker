using Brainshaker.Domain.Entities;

namespace Brainshaker.Domain.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity);
    Task<TEntity?> GetByIdAsync(Guid id);
}

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

public class UserRepository : AbstractRepository<User>
{
}