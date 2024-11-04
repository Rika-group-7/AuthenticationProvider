using System.Linq.Expressions;

namespace AuthenticationProvider.Interfaces
{
    public interface IBaseRepo<TEntity> where TEntity : class
    {
        Task<TEntity> CreateOneAsync(TEntity entity);
        Task<bool> DeleteOne(Expression<Func<TEntity, bool>> predicate);
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetOneAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> UpdateOne(TEntity entity);
    }
}