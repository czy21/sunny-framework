using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Sunny.Framework.DB.Repository;

public interface IRepositoryBase<K, T> where T : class
{
    DbSet<T> GetDbSet();

    Task<int> InsertAsync(T po, bool ignoreNull = true, bool autoCommit = true);

    Task<int> BatchInsertAsync(List<T> pos);

    Task<int> UpdateAsync(T po, bool ignoreNull = true, bool autoCommit = true);

    Task<int> UpsertAsync(T po, Dictionary<Expression<Func<T, object>>, string> updators, bool ignoreNull = true, bool autoCommit = true);

    Task<T> SelectByIdAsync(K id);

    Task<int> DeleteByIdAsync(K id);

    Task<int> SaveChangesAsync();
}