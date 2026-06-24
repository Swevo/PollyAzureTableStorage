/// <summary>Extension methods for adding Polly resilience to Azure Table Storage clients.</summary>
public static class PollyAzureTableStorageExtensions
{
    /// <summary>Wraps a <see cref="TableClient"/> with the given <see cref="ResiliencePipeline"/>.</summary>
    public static ResilientTableClient WithPolly(
        this TableClient client,
        ResiliencePipeline pipeline)
        => new(client, pipeline);

    /// <summary>Wraps a <see cref="TableClient"/> with a pipeline built by <paramref name="configure"/>.</summary>
    public static ResilientTableClient WithPolly(
        this TableClient client,
        Action<ResiliencePipelineBuilder> configure)
    {
        var builder = new ResiliencePipelineBuilder();
        configure(builder);
        return new(client, builder.Build());
    }
}
