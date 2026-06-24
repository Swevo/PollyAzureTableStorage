/// <summary>
/// Wraps a <see cref="TableClient"/> with a Polly v8 <see cref="ResiliencePipeline"/>,
/// applying retry, timeout, and circuit-breaker to every table operation.
/// </summary>
public sealed class ResilientTableClient(TableClient client, ResiliencePipeline pipeline)
{
    /// <summary>The underlying <see cref="TableClient"/>.</summary>
    public TableClient Inner => client;

    /// <summary>Adds an entity to the table, protected by the resilience pipeline.</summary>
    public Task<Response> AddEntityAsync<T>(
        T entity,
        CancellationToken cancellationToken = default)
        where T : ITableEntity
        => pipeline.ExecuteAsync(
            async ct => await client.AddEntityAsync(entity, ct),
            cancellationToken).AsTask();

    /// <summary>Upserts an entity in the table, protected by the resilience pipeline.</summary>
    public Task<Response> UpsertEntityAsync<T>(
        T entity,
        TableUpdateMode mode = TableUpdateMode.Merge,
        CancellationToken cancellationToken = default)
        where T : ITableEntity
        => pipeline.ExecuteAsync(
            async ct => await client.UpsertEntityAsync(entity, mode, ct),
            cancellationToken).AsTask();

    /// <summary>Updates an entity in the table, protected by the resilience pipeline.</summary>
    public Task<Response> UpdateEntityAsync<T>(
        T entity,
        ETag ifMatch,
        TableUpdateMode mode = TableUpdateMode.Merge,
        CancellationToken cancellationToken = default)
        where T : ITableEntity
        => pipeline.ExecuteAsync(
            async ct => await client.UpdateEntityAsync(entity, ifMatch, mode, ct),
            cancellationToken).AsTask();

    /// <summary>Deletes an entity from the table, protected by the resilience pipeline.</summary>
    public Task<Response> DeleteEntityAsync(
        string partitionKey,
        string rowKey,
        ETag ifMatch = default,
        CancellationToken cancellationToken = default)
        => pipeline.ExecuteAsync(
            async ct => await client.DeleteEntityAsync(partitionKey, rowKey, ifMatch, ct),
            cancellationToken).AsTask();

    /// <summary>Gets a single entity from the table, protected by the resilience pipeline.</summary>
    public Task<Response<T>> GetEntityAsync<T>(
        string partitionKey,
        string rowKey,
        IEnumerable<string>? select = null,
        CancellationToken cancellationToken = default)
        where T : class, ITableEntity, new()
        => pipeline.ExecuteAsync(
            async ct => await client.GetEntityAsync<T>(partitionKey, rowKey, select, ct),
            cancellationToken).AsTask();

    /// <summary>
    /// Executes any <see cref="TableClient"/> operation, protected by the resilience pipeline.
    /// </summary>
    public Task<T> ExecuteAsync<T>(
        Func<TableClient, CancellationToken, Task<T>> operation,
        CancellationToken cancellationToken = default)
        => pipeline.ExecuteAsync(
            async ct => await operation(client, ct),
            cancellationToken).AsTask();
}
