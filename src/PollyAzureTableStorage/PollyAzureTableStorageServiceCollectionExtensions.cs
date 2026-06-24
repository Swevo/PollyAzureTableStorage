/// <summary>Dependency-injection extensions for <c>PollyAzureTableStorage</c>.</summary>
public static class PollyAzureTableStorageServiceCollectionExtensions
{
    /// <summary>
    /// Registers a singleton <see cref="ResiliencePipeline"/> built by <paramref name="configure"/>
    /// and a transient <see cref="ResilientTableClient"/> wrapping the
    /// <see cref="TableClient"/> already registered in the DI container.
    /// </summary>
    public static IServiceCollection AddPollyAzureTableStorage(
        this IServiceCollection services,
        Action<ResiliencePipelineBuilder> configure)
    {
        var builder = new ResiliencePipelineBuilder();
        configure(builder);
        var pipeline = builder.Build();

        services.AddSingleton(pipeline);
        services.AddTransient<ResilientTableClient>(sp =>
            sp.GetRequiredService<TableClient>().WithPolly(pipeline));

        return services;
    }

    /// <summary>
    /// Registers a singleton <see cref="TableClient"/> for the given connection string and table name,
    /// then registers the resilience pipeline and <see cref="ResilientTableClient"/>.
    /// </summary>
    public static IServiceCollection AddPollyAzureTableStorage(
        this IServiceCollection services,
        string connectionString,
        string tableName,
        Action<ResiliencePipelineBuilder> configure)
    {
        services.AddSingleton(new TableClient(connectionString, tableName));
        return services.AddPollyAzureTableStorage(configure);
    }
}
