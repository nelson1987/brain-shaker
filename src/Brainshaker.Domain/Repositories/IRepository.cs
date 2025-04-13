namespace Brainshaker.Domain.Repositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task AddAsync(TEntity entity);
    Task<TEntity?> FindByIdAsync(int id);
}