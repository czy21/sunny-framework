using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Sunny.Framework.Core.Util;

namespace Sunny.Framework.DB.Repository;

public class RepositoryBase<K, T> : IRepositoryBase<K, T> where T : class
{
    private readonly DbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public RepositoryBase(DbContext context)
    {
        _dbContext = context;
        _dbSet = context.Set<T>();
    }

    public DbSet<T> GetDbSet()
    {
        return _dbSet;
    }

    public async Task<int> InsertAsync(T po, bool ignoreNull = true, bool autoCommit = true)
    {
        if (ignoreNull)
        {
            var entry = _dbContext.Entry(po);

            var idColumnName = entry.Property("Id").Metadata.GetColumnName();
            var primaryIsString = typeof(string) == typeof(K);

            var props = entry.Properties.Where(p => p.CurrentValue != null && (primaryIsString || p.Metadata.GetColumnName() != idColumnName)).ToList();

            var columns = string.Join(",", props.Select(p => p.Metadata.GetColumnName()));

            var values = string.Join(",", Enumerable.Range(0, props.Count).Select(t => $"@p{t}").ToList());
            var parameters = props.Select(p => p.CurrentValue ?? DBNull.Value).ToArray();
            var sql = $"INSERT INTO {entry.Metadata.GetTableName()} ({columns}) VALUES ({values});";

            if (primaryIsString) return await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);

            sql += $"SELECT LAST_INSERT_ID() as {idColumnName};";
            var result = await _dbContext.Database.SqlQueryRaw<K>(sql, parameters).ToListAsync();
            entry.Property("Id").CurrentValue = result.FirstOrDefault();
            return await Task.FromResult(result.Count).ConfigureAwait(false);
        }

        await _dbSet.AddAsync(po);
        if (autoCommit) return await SaveChangesAsync();
        return await Task.FromResult(0).ConfigureAwait(false);
    }

    public async Task<int> BatchInsertAsync(List<T> pos)
    {
        if (pos == null || pos.Count == 0) return await Task.FromResult(0).ConfigureAwait(false);

        var entityType = _dbContext.Model.FindEntityType(typeof(T));

        var tableName = entityType!.GetTableName();

        var properties = entityType!.GetProperties().Where(t => t.PropertyInfo?.Name != "Id");

        var i = 0;

        var vars = new List<string>();
        var vals = new List<object>();

        foreach (var t in pos)
        {
            vars.Add($"({string.Join(", ", properties.Select(p => $"@p{i++}"))})");
            vals.AddRange(properties.Select(p => p.PropertyInfo?.GetValue(t)));
        }

        var sql = $"INSERT INTO {tableName} ({string.Join(",", properties.Select(p => p.GetColumnName()))}) VALUES {string.Join(",", vars)}";

        return await _dbContext.Database.ExecuteSqlRawAsync(sql, [.. vals]);
    }

    public async Task<int> UpdateAsync(T po, bool ignoreNull = true, bool autoCommit = true)
    {
        if (ignoreNull)
        {
            var entry = _dbContext.Entry(po);

            var idValue = entry.Property("Id").CurrentValue;

            if (idValue == null) return await Task.FromResult(0).ConfigureAwait(false);

            var idColumnName = entry.Property("Id").Metadata.GetColumnName();

            var props = entry.Properties.Where(p => p.CurrentValue != null && p.Metadata.GetColumnName() != idColumnName).ToList();

            var values = string.Join(",", Enumerable.Range(0, props.Count).Select(t => $"set {props[t].Metadata.GetColumnName()}=@p{t + 1}").ToList());

            var sql = $"update {entry.Metadata.GetTableName()} {values} where {idColumnName} = @p0";

            var parameters = props.Select(p => p.CurrentValue ?? DBNull.Value).ToArray();

            return await _dbContext.Database.ExecuteSqlRawAsync(sql, [idValue, .. parameters]);
        }

        _dbSet.Update(po);
        if (autoCommit) return await SaveChangesAsync();
        return await Task.FromResult(0).ConfigureAwait(false);
    }

    public async Task<int> UpsertAsync(T po, Dictionary<Expression<Func<T, object>>, string> updators, bool ignoreNull = true, bool autoCommit = true)
    {
        var entry = _dbContext.Entry(po);

        var idColumnName = entry.Property("Id").Metadata.GetColumnName();
        var primaryIsString = typeof(string) == typeof(K);

        var props = entry.Properties.Where(p => p.CurrentValue != null && (primaryIsString || p.Metadata.GetColumnName() != idColumnName)).ToList();

        var columns = string.Join(",", props.Select(p => p.Metadata.GetColumnName()));

        var values = string.Join(",", Enumerable.Range(0, props.Count).Select(t => $"@p{t}").ToList());

        var updateSets = new Dictionary<string, string>();

        if (!primaryIsString) updateSets[idColumnName] = $"LAST_INSERT_ID({idColumnName})";

        foreach (var u in updators)
        {
            var propertyName = ReflectionUtil.GetPropertyName(u.Key);
            var columnName = props.Where(t => t.Metadata.Name == propertyName)?.FirstOrDefault()?.Metadata.GetColumnName();
            if (columnName != null) updateSets[columnName] = u.Value;
        }

        var updateSetStr = string.Join(",", updateSets.Select(t => $"{t.Key} = {t.Value}"));
        var sql = $"INSERT INTO {entry.Metadata.GetTableName()} ({columns}) VALUES ({values}) ON DUPLICATE KEY UPDATE {updateSetStr};";

        var parameters = props.Select(p => p.CurrentValue ?? DBNull.Value).ToArray();

        if (primaryIsString) return await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);
        sql += $"SELECT LAST_INSERT_ID() as {idColumnName};";
        var result = await _dbContext.Database.SqlQueryRaw<K>(sql, parameters).ToListAsync();
        entry.Property("Id").CurrentValue = result.FirstOrDefault();
        return await Task.FromResult(result.Count).ConfigureAwait(false);
    }

    public async Task<T> SelectByIdAsync(K id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<int> DeleteByIdAsync(K id)
    {
        return await _dbSet.Where(t => EF.Property<K>(t, "Id").Equals(id)).ExecuteDeleteAsync();
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}